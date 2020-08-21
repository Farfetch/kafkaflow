namespace KafkaFlow.Client.Protocol
{
    using System;
    using System.Buffers.Binary;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using KafkaFlow.Client.Protocol.Messages;

    internal static class StreamExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteMessage(this Stream destination, IRequest message) => message.Write(destination);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt16(this Stream destination, short value)
        {
            Span<byte> tmp = stackalloc byte[2];
            BinaryPrimitives.WriteInt16BigEndian(tmp, value);
            destination.Write(tmp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32(this Stream destination, int value)
        {
            Span<byte> tmp = stackalloc byte[4];
            BinaryPrimitives.WriteInt32BigEndian(tmp, value);
            destination.Write(tmp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64(this Stream destination, long value)
        {
            Span<byte> tmp = stackalloc byte[8];
            BinaryPrimitives.WriteInt64BigEndian(tmp, value);
            destination.Write(tmp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32Array(this Stream destination, IReadOnlyList<int> values)
        {
            destination.WriteInt32(values.Count);
            InternalWriteInt32Array(destination, values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteCompactInt32Array(this Stream destination, IReadOnlyList<int> values)
        {
            destination.WriteUVarint((uint) values.Count);
            InternalWriteInt32Array(destination, values);
        }

        private static void InternalWriteInt32Array(Stream destination, IReadOnlyList<int> values)
        {
            for (var i = 0; i < values.Count; ++i)
            {
                destination.WriteInt32(values[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteString(this Stream destination, string value)
        {
            if (value is null)
            {
                destination.WriteInt16(-1);
                return;
            }

            destination.WriteInt16(Convert.ToInt16(value.Length));
            destination.Write(Encoding.UTF8.GetBytes(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteCompactString(this Stream destination, string value)
        {
            destination.WriteUVarint((uint) value.Length + 1u);
            destination.Write(Encoding.UTF8.GetBytes(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteCompactNullableString(this Stream destination, string? value)
        {
            if (value is null)
            {
                destination.WriteUVarint(0);
                return;
            }

            destination.WriteCompactString(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteCompactNullableByteArray(this Stream destination, byte[]? data)
        {
            if (data is null)
            {
                destination.WriteUVarint(0);
                return;
            }

            destination.WriteCompactByteArray(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteCompactByteArray(this Stream destination, byte[] data)
        {
            destination.WriteUVarint((uint) data.Length + 1u);
            destination.Write(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBoolean(this Stream destination, bool value)
        {
            destination.WriteByte((byte) (value ? 1 : 0));
        }

        public static void WriteArray<TMessage>(this Stream destination, TMessage[] items)
            where TMessage : IRequest
        {
            destination.WriteInt32(items.Length);

            for (var i = 0; i < items.Length; ++i)
            {
                destination.WriteMessage(items[i]);
            }
        }

        public static void WriteArray<TMessage>(this Stream destination, IReadOnlyCollection<TMessage> items)
            where TMessage : IRequest
        {
            destination.WriteInt32(items.Count);

            foreach (var item in items)
            {
                destination.WriteMessage(item);
            }
        }

        public static void WriteTaggedFields(this Stream destination, TaggedField[] items)
        {
            destination.WriteUVarint((uint) items.Length);

            for (var i = 0; i < items.Length; ++i)
            {
                destination.WriteMessage(items[i]);
            }
        }

        public static void WriteCompactArray<TMessage>(this Stream destination, TMessage[] items)
            where TMessage : IRequest
        {
            destination.WriteUVarint((uint) items.Length + 1);

            for (var i = 0; i < items.Length; ++i)
            {
                destination.WriteMessage(items[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolean(this Stream source) => source.ReadByte() != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ErrorCode ReadErrorCode(this Stream source) => (ErrorCode) source.ReadInt16();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(this Stream source)
        {
            Span<byte> buffer = stackalloc byte[2];
            source.Read(buffer);
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(this Stream source)
        {
            Span<byte> buffer = stackalloc byte[4];
            source.Read(buffer);
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(this Stream source)
        {
            Span<byte> buffer = stackalloc byte[8];
            source.Read(buffer);
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadBytes(this Stream source, int count)
        {
            var bytes = new byte[count];
            source.Read(bytes);
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? ReadString(this Stream source)
        {
            return source.ReadString(source.ReadInt16());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? ReadNullableString(this Stream source)
        {
            return source.ReadNullableString(source.ReadInt16());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? ReadNullableString(this Stream source, int size)
        {
            return size < 0 ? null : source.ReadString(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(this Stream source, int size)
        {
            Span<byte> buffer = stackalloc byte[size];
            source.Read(buffer);
            return Encoding.UTF8.GetString(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? ReadCompactNullableString(this Stream source)
        {
            var size = source.ReadUVarint();

            return size <= 0 ?
                null :
                source.ReadString(size - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadCompactString(this Stream source)
        {
            return source.ReadString(source.ReadUVarint() - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadCompactByteArray(this Stream source)
        {
            var size = source.ReadUVarint();

            if (size <= 0)
                return null;

            return source.ReadBytes(size - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TMessage ReadMessage<TMessage>(this Stream source)
            where TMessage : class, IResponse, new()
        {
            var message = new TMessage();
            message.Read(source);
            return message;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TMessage[] ReadArray<TMessage>(this Stream source) where TMessage : class, IResponse, new() =>
            source.ReadArray<TMessage>(source.ReadInt32());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TMessage[] ReadCompactArray<TMessage>(this Stream source) where TMessage : class, IResponse, new() =>
            source.ReadArray<TMessage>(source.ReadUVarint() - 1);

        public static TMessage[] ReadArray<TMessage>(this Stream source, int count) where TMessage : class, IResponse, new()
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

        public static TaggedField[] ReadTaggedFields(this Stream source)
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
        public static int[] ReadInt32Array(this Stream source) => source.ReadInt32Array(source.ReadInt32());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ReadCompactInt32Array(this Stream source) => source.ReadInt32Array(source.ReadUVarint() - 1);

        public static int[] ReadInt32Array(this Stream source, int count)
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
        public static int ReadVarint(this Stream source)
        {
            var num = source.ReadUVarint();

            return (num >> 1) ^ -(num & 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadUVarint(this Stream source) => source.ReadUVarint(out _);

        public static int ReadUVarint(this Stream source, out int bytesRead)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteVarint(this Stream destination, long num) =>
            destination.WriteUVarint(((ulong) num << 1) ^ ((ulong) num >> 63));

        public static void WriteUVarint(this Stream destination, ulong num)
        {
            const ulong endMask = 0b1000_0000;
            const ulong valueMask = 0b0111_1111;

            do
            {
                var value = (byte) ((num & valueMask) | (num > valueMask ? endMask : 0));
                destination.WriteByte(value);
                num >>= 7;
            } while (num != 0);
        }
    }
}
