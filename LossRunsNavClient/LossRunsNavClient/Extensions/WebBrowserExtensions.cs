using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LossRunsNavClient
{
    public static class WebBrowserExtensions
    {
        public static void UntilLoaded(this WebBrowser browser, AutoResetEvent monitor)
        {
            browser.UntilLoaded(1000, 60, monitor);
        }

        public static void UntilLoaded(this WebBrowser browser, int millisTimeout, int numberOfTimouts, AutoResetEvent monitor)
        {
            int halfNumberOfTimeouts = numberOfTimouts / 2;

            int timeoutsWaited = 0;

            while (!monitor.WaitOne(millisTimeout))
            {
                timeoutsWaited += 1;
                if(timeoutsWaited > halfNumberOfTimeouts)
                {
                    if (browser.ReadyState == WebBrowserReadyState.Complete) { return; }
                }
                Application.DoEvents();
            }
            if (browser.ReadyState == WebBrowserReadyState.Complete) { return; }

            throw new TimeoutException();
        }



        public static void ClearEventHandlers(this WebBrowser browser, string eventName)
        {
            FieldInfo eventField = browser.GetType().GetEventField(eventName);
            eventField.SetValue(browser, null);
        }

        public static FieldInfo GetEventField(this Type type, string eventName)
        {
            FieldInfo backingField = null;
            while (type != null)
            {
                backingField = type.GetField(eventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (backingField != null && (backingField.FieldType == typeof(MulticastDelegate) || backingField.FieldType.IsSubclassOf(typeof(MulticastDelegate))))
                {
                    break;
                }

                backingField = type.GetField("EVENT_" + eventName.ToUpper(), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (backingField != null) { break; }
                type = type.BaseType;
            }
            return backingField;
        }
    }
}
