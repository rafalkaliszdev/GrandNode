using Grand.Core;
using Grand.Core.Data;
using Grand.Core.Infrastructure;

namespace Grand.Data
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
            var settings = EngineContextExperimental.Current.Resolve<DataSettings>();
            if (settings != null && settings.IsValid())
            {
                var provider = EngineContextExperimental.Current.Resolve<IDataProvider>();
                if (provider == null)
                    throw new GrandException("No IDataProvider found");

                provider.SetDatabaseInitializer();
            }
        }

        public int Order
        {
            //ensure that this task is run first 
            get { return -1000; }
        }
    }
}
