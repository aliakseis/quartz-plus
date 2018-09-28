namespace ILC_ControlPanel
{
    /// <summary>
    /// Class implements log setting
    /// </summary>
    class LogSetting
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name">setting name</param>
        /// <param name="val">setting value</param>
        public LogSetting(string name, string val)
        {
            this.name = name;
            this.val = val;
        }

        private string name;
        /// <summary>
        /// setting name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string val;
        /// <summary>
        /// setting value
        /// </summary>
        public string Val
        {
            get { return val; }
            set { val = value; }
        }
    }
}