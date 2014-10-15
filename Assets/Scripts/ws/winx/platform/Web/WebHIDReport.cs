using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ws.winx.platform.web
{
	public class WebHIDReport:HIDReport
	{
        private List<object> _buttons;

        public List<object> buttons
        {
            get { return _buttons; }
            set { _buttons = value; }
        }

        private List<object> _axes;

        public List<object> axes
        {
            get { return _axes; }
            set { _axes = value; }
        }

            public WebHIDReport(int index,byte[] data, ReadStatus status):base(index,data,ReadStatus.NoDataRead)
            {
               
            }

	}
}
