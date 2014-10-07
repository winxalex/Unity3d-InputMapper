using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ws.winx.platform
{
	public class HIDReport
	{
        protected int _index;

            public enum ReadStatus
            {
                Success = 0,
                WaitTimedOut = 1,
                WaitFail = 2,
                NoDataRead = 3,
                ReadError = 4,
                NotConnected = 5,
                Refresh = 6,
                Resent
            }

            public HIDReport()
            {
               
                Status = ReadStatus.Success;
            }

            public HIDReport(ReadStatus status)
            {
                Data = new byte[] { };
                Status = status;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="index"> of GenericHIDDevice from which this report comes</param>
            /// <param name="data"></param>
            /// <param name="status"></param>
            public HIDReport(int index,byte[] data, ReadStatus status)
            {
                this._index = index;
                Data = data;
                Status = status;
            }

            public byte[] Data { get; internal set; }
            public ReadStatus Status { get; internal set; }


            public int index { get { return _index; } internal set {_index=value;} }
    }
}
