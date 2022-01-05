namespace KafkaFlow.Client.Protocol.Security.Authentication.SASL.Scram
{
    using global::System;
    using global::System.Buffers.Binary;
    using global::System.Diagnostics;
    using global::System.Security.Cryptography;
    using global::System.Text;

    public class Rfc2898DeriveBytes : DeriveBytes
    {
        private readonly int blockSize;
        private readonly byte[] salt;
        private readonly uint iterations;
        private HMAC hmac;

        private byte[] buffer;
        private uint block;
        private int startIndex;
        private int endIndex;

        /// <summary>
        /// Gets the hash algorithm used for byte derivation.
        /// </summary>
        public HashAlgorithmName HashAlgorithm { get; }

        public Rfc2898DeriveBytes(string password, byte[] salt, int iterations, HashAlgorithmName hashAlgorithm)
            : this(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                clearPassword: true)
        {
        }

        internal Rfc2898DeriveBytes(
            byte[] password,
            byte[] salt,
            int iterations,
            HashAlgorithmName hashAlgorithm,
            bool clearPassword)
        {
            if (salt is null)
            {
                throw new ArgumentNullException(nameof(salt));
            }

            if (iterations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(iterations));
            }

            if (password is null)
            {
                throw
                    new NullReferenceException(); // This "should" be ArgumentNullException but for compat, we throw NullReferenceException.
            }

            this.salt = new byte[salt.Length + sizeof(uint)];
            salt.AsSpan().CopyTo(this.salt);
            this.iterations = (uint)iterations;
            this.HashAlgorithm = hashAlgorithm;
            this.hmac = this.OpenHmac(password);

            Array.Clear(password, 0, password.Length);

            // _blockSize is in bytes, HashSize is in bits.
            this.blockSize = this.hmac.HashSize >> 3;
            this.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.hmac.Dispose();
                this.hmac = null!;

                Array.Clear(this.buffer, 0, this.buffer.Length);
                Array.Clear(this.salt, 0, this.salt.Length);
            }

            base.Dispose(disposing);
        }

        public override byte[] GetBytes(int cb)
        {
            if (cb <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cb));
            }

            var password = new byte[cb];

            var offset = 0;
            var size = this.endIndex - this.startIndex;
            if (size > 0)
            {
                if (cb >= size)
                {
                    Buffer.BlockCopy(
                        this.buffer,
                        this.startIndex,
                        password,
                        0,
                        size);
                    this.startIndex = this.endIndex = 0;
                    offset += size;
                }
                else
                {
                    Buffer.BlockCopy(
                        this.buffer,
                        this.startIndex,
                        password,
                        0,
                        cb);
                    this.startIndex += cb;
                    return password;
                }
            }

            Debug.Assert(this.startIndex == 0 && this.endIndex == 0, "Invalid start or end index in the internal buffer.");

            while (offset < cb)
            {
                this.Func();
                var remainder = cb - offset;
                if (remainder >= this.blockSize)
                {
                    Buffer.BlockCopy(
                        this.buffer,
                        0,
                        password,
                        offset,
                        this.blockSize);
                    offset += this.blockSize;
                }
                else
                {
                    Buffer.BlockCopy(
                        this.buffer,
                        0,
                        password,
                        offset,
                        remainder);
                    this.startIndex = remainder;
                    this.endIndex = this.buffer.Length;
                    return password;
                }
            }

            return password;
        }

        public override void Reset()
        {
            this.Initialize();
        }

        private HMAC OpenHmac(byte[] password)
        {
            Debug.Assert(password != null);

            var hashAlgorithm = this.HashAlgorithm;

            if (string.IsNullOrEmpty(hashAlgorithm.Name))
            {
                throw new CryptographicException();
            }

            if (hashAlgorithm == HashAlgorithmName.SHA1)
            {
                return new HMACSHA1(password);
            }

            if (hashAlgorithm == HashAlgorithmName.SHA256)
            {
                return new HMACSHA256(password);
            }

            if (hashAlgorithm == HashAlgorithmName.SHA384)
            {
                return new HMACSHA384(password);
            }

            if (hashAlgorithm == HashAlgorithmName.SHA512)
            {
                return new HMACSHA512(password);
            }

            throw new CryptographicException();
        }

        private void Initialize()
        {
            Array.Clear(this.buffer, 0, this.buffer.Length);

            this.buffer = new byte[this.blockSize];
            this.block = 0;
            this.startIndex = this.endIndex = 0;
        }

        // This function is defined as follows:
        // Func (S, i) = HMAC(S || i) ^ HMAC2(S || i) ^ ... ^ HMAC(iterations) (S || i)
        // where i is the block number.
        private void Func()
        {
            // Block number is going to overflow, exceeding the maximum total possible bytes
            // that can be extracted.
            if (this.block == uint.MaxValue)
            {
                throw new CryptographicException();
            }

            BinaryPrimitives.WriteUInt32BigEndian(this.salt.AsSpan(this.salt.Length - sizeof(uint)), this.block + 1);

            var uiSpan = this.hmac.ComputeHash(this.salt);
            uiSpan.AsSpan().CopyTo(this.buffer);

            for (var i = 2; i <= this.iterations; i++)
            {
                uiSpan = this.hmac.ComputeHash(uiSpan);

                for (var j = this.buffer.Length - 1; j >= 0; j--)
                {
                    this.buffer[j] ^= uiSpan[j];
                }
            }

            // increment the block count.
            this.block++;
        }
    }
}
