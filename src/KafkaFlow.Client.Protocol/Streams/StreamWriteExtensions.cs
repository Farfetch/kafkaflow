namespace KafkaFlow.Client.Protocol.Streams
{
    using System;
    using System.Buffers.Binary;
    using System.Collections.Generic;
    using System.Text;
    using KafkaFlow.Client.Protocol.Messages;

    internal static class StreamWriteExtensions
    {
        internal static void WriteMessage(this MemoryWriter destination, IRequest message) => message.Write(destination);

        internal static void WriteInt16(this MemoryWriter destination, short value)
        {
            const int byteCount = 2;

            if (destination.TryGetSpan(byteCount, out var span))
            {
                BinaryPrimitives.WriteInt16BigEndian(span, value);
                destination.Advance(byteCount);
                return;
            }

            Span<byte> tmp = stackalloc byte[byteCount];
            BinaryPrimitives.WriteInt16BigEndian(tmp, value);
            destination.Write(tmp);
        }

        internal static void WriteByte(this MemoryWriter destination, byte value)
        {
            const int byteCount = 1;

            if (destination.TryGetSpan(byteCount, out var span))
            {
                span[0] = value;
                destination.Advance(byteCount);
                return;
            }

            Span<byte> tmp = stackalloc byte[byteCount];
            tmp[0] = value;
            destination.Write(tmp);
        }

        internal static void WriteInt32(this MemoryWriter destination, int value)
        {
            const int byteCount = 4;

            if (destination.TryGetSpan(byteCount, out var span))
            {
                BinaryPrimitives.WriteInt32BigEndian(span, value);
                destination.Advance(byteCount);
                return;
            }

            Span<byte> tmp = stackalloc byte[byteCount];
            BinaryPrimitives.WriteInt32BigEndian(tmp, value);
            destination.Write(tmp);
        }

        internal static void WriteInt64(this MemoryWriter destination, long value)
        {
            const int byteCount = 8;

            if (destination.TryGetSpan(byteCount, out var span))
            {
                BinaryPrimitives.WriteInt64BigEndian(span, value);
                destination.Advance(byteCount);
                return;
            }

            Span<byte> tmp = stackalloc byte[byteCount];
            BinaryPrimitives.WriteInt64BigEndian(tmp, value);
            destination.Write(tmp);
        }

        internal static void WriteInt32Array(this MemoryWriter destination, IReadOnlyList<int> values)
        {
            destination.WriteInt32(values.Count);
            InternalWriteInt32Array(destination, values);
        }

        private static void InternalWriteInt32Array(MemoryWriter destination, IReadOnlyList<int> values)
        {
            foreach (var t in values)
            {
                destination.WriteInt32(t);
            }
        }

        internal static void WriteString(this MemoryWriter destination, string? value)
        {
            if (value is null)
            {
                destination.WriteInt16(-1);
                return;
            }

            destination.WriteInt16(Convert.ToInt16(value.Length));
            destination.WriteRawString(value);
        }

        internal static void WriteCompactString(this MemoryWriter destination, string value)
        {
            destination.WriteUVarint((uint)value.Length + 1u);
            destination.WriteRawString(value);
        }

        private static void WriteRawString(this MemoryWriter destination, string value)
        {
            const int maxUtf8CharCount = 4;

            if (destination.TryGetSpan(value.Length * maxUtf8CharCount, out var span))
            {
                destination.Advance(Encoding.UTF8.GetBytes(value, span));
                return;
            }

            destination.Write(Encoding.UTF8.GetBytes(value));
        }

        internal static void WriteBoolean(this MemoryWriter destination, bool value)
        {
            destination.WriteByte((byte)(value ? 1 : 0));
        }

        private static void WriteArray<TMessage>(this MemoryWriter destination, IEnumerable<TMessage> items, int count)
            where TMessage : IRequest
        {
            destination.WriteInt32(count);

            foreach (var item in items)
            {
                destination.WriteMessage(item);
            }
        }

        internal static void WriteArray<TMessage>(this MemoryWriter destination, IReadOnlyCollection<TMessage> items)
            where TMessage : IRequest
        {
            destination.WriteArray(items, items.Count);
        }

        internal static void WriteTaggedFields(this MemoryWriter destination, IReadOnlyList<TaggedField> items)
        {
            destination.WriteUVarint((uint)items.Count);

            foreach (var t in items)
            {
                destination.WriteMessage(t);
            }
        }

        internal static void WriteCompactArray<TMessage>(this MemoryWriter destination, IReadOnlyList<TMessage> items)
            where TMessage : IRequest
        {
            destination.WriteUVarint((uint)items.Count + 1);

            foreach (var t in items)
            {
                destination.WriteMessage(t);
            }
        }

        internal static void WriteUVarint(this MemoryWriter destination, ulong num)
        {
            const int byteCount = 8;

            if (destination.TryGetSpan(byteCount, out var span))
            {
                destination.Advance(WriteUVarint(num, span));
                return;
            }

            Span<byte> buffer = stackalloc byte[byteCount];
            var bytesUsed = WriteUVarint(num, buffer);
            destination.Write(buffer[..bytesUsed]);
        }

        private static int WriteUVarint(ulong num, Span<byte> buffer)
        {
            const ulong endMask = 0b1000_0000;
            const ulong valueMask = 0b0111_1111;

            var bytesWritten = 0;

            do
            {
                var value = (byte)((num & valueMask) | (num > valueMask ? endMask : 0));
                buffer[bytesWritten++] = value;
                num >>= 7;
            }
            while (num != 0);

            return bytesWritten;
        }
    }
}
