using System;

namespace FCSPlayout.Domain
{
    public class DefaultDateTimeService : IDateTimeService
    {
        private static readonly DefaultDateTimeService _instance = new DefaultDateTimeService();

        private DefaultDateTimeService()
        {

        }

        public static DefaultDateTimeService Instance
        {
            get
            {
                return _instance;
            }
        }

        public DateTime GetLocalNow()
        {
            return DateTime.Now;
        }
    }
}
