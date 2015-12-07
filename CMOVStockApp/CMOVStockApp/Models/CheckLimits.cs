using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace CMOVStockApp.Models
{
    class CheckLimits
    {
        public static async void checkIntervals()
        {
            await YahooFinance.getQuotes();

            for (int i=0;i<YahooFinance.observingCompanies.Count;i++)
            {
                if(YahooFinance.observingCompanies.ElementAt(i).quote <YahooFinance.observingCompanies.ElementAt(i).min)
                {
                    
                }
                else if (YahooFinance.observingCompanies.ElementAt(i).quote > YahooFinance.observingCompanies.ElementAt(i).max)
                {

                }
            }
        }
    }
}
