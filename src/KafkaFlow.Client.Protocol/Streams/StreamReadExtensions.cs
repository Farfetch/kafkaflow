namespace KafkaFlow.Client.Protocol.Streams
{
    using System;
    using System.Buffers.Binary;
    using System.Runtime.CompilerServices;
    using System.Text;
    using KafkaFlow.Client.Protocol.Messages;

    public static class StreamReadExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolean(this BaseMemoryStream source) => source.ReadByte() != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ErrorCode ReadErrorCode(this BaseMemoryStream source) => (ErrorCode) source.ReadInt16();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(this BaseMemoryStream source)
        {
            Span<byte> buffer = stackalloc byte[2];
            source.Read(buffer);
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(this BaseMemoryStream source)
        {
            Span<byte> buffer = stackalloc byte[4];
            source.Read(buffer);
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(this BaseMemoryStream source)
        {
            Span<byte> buffer = stackalloc byte[8];
            source.Read(buffer);
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadBytes(this BaseMemoryStream source, int count)
        {
            var bytes = new byte[count];
            source.Read(bytes);
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? ReadString(this BaseMemoryStream source)
        {
            return source.ReadString(source.ReadInt16());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? ReadNullableString(this BaseMemoryStream source)
        {
            return source.ReadNullableString(source.ReadInt16());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? ReadNullableString(this BaseMemoryStream source, int size)
        {
            return size < 0 ? null : source.ReadString(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(this BaseMemoryStream source, int size)
        {
            Span<byte> buffer = stackalloc byte[size];
            source.Read(buffer);
            return Encoding.UTF8.GetString(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? ReadCompactNullableString(this BaseMemoryStream source)
        {
            var size = source.ReadUVarint();

            return size <= 0 ?
                null :
                source.ReadString(size - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadCompactString(this BaseMemoryStream source)
        {
            return source.ReadString(source.ReadUVarint() - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadCompactByteArray(this BaseMemoryStream source)
        {
            var size = source.ReadUVarint();

            if (size <= 0)
                return null;

            return source.ReadBytes(size - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TMessage ReadMessage<TMessage>(this BaseMemoryStream source)
            where TMessage : class, IResponse, new()
        {
            var message = new TMessage();
            message.Read(source);
            return message;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TMessage[] ReadArray<TMessage>(this BaseMemoryStream source) where TMessage : class, IResponse, new() =>
            source.ReadArray<TMessage>(source.ReadInt32());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TMessage[] ReadCompactArray<TMessage>(this BaseMemoryStream source) where TMessage : class, IResponse, new() =>
            source.ReadArray<TMessage>(source.ReadUVarint() - 1);

        public static TMessage[] ReadArray<TMessage>(this BaseMemoryStream source, int count) where TMessage : class, IResponse, new()
        {
            if (count < 0)
                return null;

            if (count == 0)
                return Array.Empty<TMessage>();

            var result = new TMessage[count];

            for (var i = 0; i < count; i++)
            {
                result[i] = new TMessage();
                result[i].Read(source);
            }

            return result;
        }

        public static TaggedField[] ReadTaggedFields(this BaseMemoryStream source)
        {
            var count = source.ReadUVarint();

            if (count == 0)
                return Array.Empty<TaggedField>();

            var result = new TaggedField[count];

            for (var i = 0; i < count; i++)
            {
                result[i] = new TaggedField();
                result[i].Read(source);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ReadInt32Array(this BaseMemoryStream source) => source.ReadInt32Array(source.ReadInt32());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ReadCompactInt32Array(this BaseMemoryStream source) => source.ReadInt32Array(source.ReadUVarint() - 1);

        public static int[] ReadInt32Array(this BaseMemoryStream source, int count)
        {
            if (count < 0)
                return null;

            if (count == 0)
                return Array.Empty<int>();

            var result = new int[count];

            for (var i = 0; i < count; ++i)
            {
                result[i] = source.ReadInt32();
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadVarint(this BaseMemoryStream source)
        {
            var num = source.ReadUVarint();

            return (num >> 1) ^ -(num & 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadUVarint(this BaseMemoryStream source) => source.ReadUVarint(out _);

        public static int ReadUVarint(this BaseMemoryStream source, out int bytesRead)
        {
            const int endMask = 0b1000_0000;
            const int valueMask = 0b0111_1111;

            bytesRead = 0;

            var num = 0;
            var shift = 0;
            int current;

            do
            {
                current = source.ReadByte();

                if (++bytesRead > 4)
                    throw new InvalidOperationException("The value is not a valid VARINT");

                num |= (current & valueMask) << shift;
                shift += 7;
            } while ((current & endMask) != 0);

            return num;
        }
    }
}
