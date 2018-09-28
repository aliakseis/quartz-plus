using System;
using System.Text;

namespace ILCWebApplication
{
    /// <summary>
    /// Implements getting of general constant expressions used in code
    /// </summary>
    public static class ConstExpressions
    {
        /// <summary>
        /// Regular expression for email address validation
        /// (source: http://www.regular-expressions.info/email.html)
        /// </summary>
        public const string SINGLE_EMAIL_REGEXP =
            @"[\s]*[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?[\s]*";

        /// <summary>
        /// Regular expression for comma separated email addresses validation
        /// </summary>
        public const string MULTI_EMAIL_REGEXP = "(" + SINGLE_EMAIL_REGEXP + ")(,(" + SINGLE_EMAIL_REGEXP + "))*";
        
        /// <summary>
        /// Gets decode check expression for casting values to bool values in DB queries
        /// </summary>
        /// <param name="strField">table field name</param>
        /// <param name="strValue">field value</param>
        /// <returns></returns>
        public static string GetCheckExpression(string strField, string strValue)
        {
            return string.Format("decode( {0}, {1}, 'False', 'True')", strField, strValue);
        }

        /// <summary>
        /// Gets web application name
        /// </summary>
        /// <returns>web application name</returns>
        public static string GetWebApplicationName()
        {
            return "IVRS Line Checker Administrator";
        }

        /// <summary>
        /// Gets a lookup calendar expression for the date input
        /// </summary>
        /// <returns>lookup calendar expression</returns>
        public static string GetDateExpression(string name, string imagesPath)
        {
            StringBuilder result = new StringBuilder();
            String calendarVar = name + "CalendarVar";
            String calendarDiv = name + "CalendarDiv";
            String calendarImageDiv = name + "CalendarImage";

            result
                .Append("<a href=\"javascript:")
                .Append(calendarVar)
                .Append(".showHideCalendar()\"><img src=\"")
                .Append(imagesPath)
                .Append("icon_calendar.gif\" ")
                .Append("id=\"")
                .Append(calendarImageDiv)
                .Append("\" ")
                .Append("border=\"0\" style=\"position:relative;\" /></a>")
                
                .Append("<div id=\"")
                .Append(calendarDiv)
                .Append("\" class=\"calPopupExt\" style=\"visibility:hidden\" ")
                .Append("onMouseOver=\"")
                .Append(calendarVar)
                .Append(".mouseOver()\" onMouseOut=\"")
                .Append(calendarVar)
                .Append(".mouseOut()\" ></div>")

                .Append("<script type=\"text/javascript\"> var ")
                .Append(calendarVar)
                .Append(" = new CalendarExt(\"")
                .Append(calendarDiv)
                .Append("\",\"")
                .Append(calendarVar)
                .Append("\",document.getElementById(\"")
                .Append(name)
                .Append("\"),\"")
                .Append(imagesPath)
                .Append("\",\"")
                .Append(calendarImageDiv)
                .Append("\"); </script>");

            return result.ToString();
        }

        /// <summary>
        /// Gets a lookup clock expression for the time input
        /// </summary>
        /// <returns>lookup clock expression</returns>
        public static string GetTimeExpression(string name, string imagesPath)
        {
            StringBuilder result = new StringBuilder();
            String clockVar = name + "ClockVar";
            String clockDiv = name + "ClockDiv";
            String clockImageDiv = name + "ClockImage";

            result
                .Append("<a href=\"javascript:")
                .Append(clockVar)
                .Append(".showHideClock()\"><img src=\"")
                .Append(imagesPath)
                .Append("clock.gif\" ")
                .Append("id=\"")
                .Append(clockImageDiv)
                .Append("\" ")
                .Append("border=\"0\" style=\"position:relative;\" /></a>")

                .Append("<div id=\"")
                .Append(clockDiv)
                .Append("\" class=\"clockStyle\" style=\"visibility:hidden\" ")
                .Append("onMouseOver=\"")
                .Append(clockVar)
                .Append(".mouseOver()\" onMouseOut=\"")
                .Append(clockVar)
                .Append(".mouseOut()\" ></div>")

                .Append("<script type=\"text/javascript\"> var ")
                .Append(clockVar)
                .Append(" = new Clock(")
                .Append("document.getElementById(\'")
                .Append(name)
                .Append("\') ,\"")
                .Append(clockDiv)
                .Append("\", \"")
                .Append(imagesPath)
                .Append("\", \"")
                .Append(clockVar)
                .Append("\", \"")
                .Append(clockImageDiv)
                .Append("\");")
                .Append("</script>");

            return result.ToString();
        }

        public const string START_BUTTON_TEXT = "Start";
        public const string STOP_BUTTON_TEXT = "Stop";
        public const string PAUSED_BUTTON_TEXT = "Paused...";

        public const string SERVICE_STATUS_STARTED = "Started";
        public const string SERVICE_STATUS_STOPPED = "Stopped";
        public const string SERVICE_STATUS_PAUSED = "Paused";

        public const string WORK_STATUS_IDLE = "Idle";

    }
}
