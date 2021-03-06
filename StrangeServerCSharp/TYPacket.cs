using System.Text;

namespace StrangeServerCSharp
{
    public struct TYPacket
    {
        public const int eventTypeLength = 4;
        public const int eventTimeLength = 4;
        public const int eventLocationLength = 4;

        public readonly string eventType;
        public readonly uint eventTime;
        public readonly uint x;
        public readonly uint y;
        public readonly byte[] data;

        public TYPacket(byte[] input)
        {
            byte[] et = new byte[eventTypeLength], t = new byte[eventTimeLength], loc = new byte[eventLocationLength];
            Buffer.BlockCopy(input, 0, et, 0, et.Length);
            Buffer.BlockCopy(input, et.Length, t, 0, t.Length);
            Buffer.BlockCopy(input, et.Length + t.Length, loc, 0, loc.Length);
            x = BitConverter.ToUInt32(loc);
            var headerLength = et.Length + t.Length + loc.Length * 2;
            Buffer.BlockCopy(input, headerLength - loc.Length, loc, 0, loc.Length);
            y = BitConverter.ToUInt32(loc);
            data = new byte[input.Length - headerLength];
            Buffer.BlockCopy(input, headerLength, data, 0, data.Length);
            eventType = Encoding.UTF8.GetString(et);
            eventTime = BitConverter.ToUInt32(t);
        }

        public TYPacket(uint eventTime, string eventType, uint x, uint y, byte[] data)
        {
            this.eventTime = eventTime;
            this.eventType = eventType;
            this.x = x;
            this.y = y;
            this.data = data;
        }

        public TYPacket(uint eventTime, string eventType, uint x, uint y, string data) : this(eventTime, eventType, x,
            y, Encoding.UTF8.GetBytes(data))
        {
        }

        public byte[] Compile
        {
            get
            {
                var ret = new byte[Length];
                if (eventType.Length != eventTypeLength) throw new Exception("Invalid event type length");
                var a = Encoding.UTF8.GetBytes(eventType);
                var b = BitConverter.GetBytes(eventTime);
                var c = BitConverter.GetBytes(x);
                var d = BitConverter.GetBytes(y);
                Buffer.BlockCopy(a, 0, ret, ret.Length, a.Length);
                Buffer.BlockCopy(b, 0, ret, ret.Length, b.Length);
                Buffer.BlockCopy(c, 0, ret, ret.Length, c.Length);
                Buffer.BlockCopy(d, 0, ret, ret.Length, d.Length);
                Buffer.BlockCopy(data, 0, ret, ret.Length, data.Length);
                return ret;
            }
        }

        public byte[] RawCompile => new Packet("B", "TY", Compile).Compile;

        public uint Length => (uint)(eventTime.ToString().Length + eventType.Length + x.ToString().Length +
                                     y.ToString().Length + data.Length);
    }
}