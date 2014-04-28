using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nJupiter.Globalization.Tests
{
    [TestClass]
    public class ConfigurableCultureHandlerTests
    {
        [TestMethod]
        public void Convert1000IntsToCurrencyStrings_Using_nJupiterGlobalization_CurrentUICulture() {
            var instance = CultureHandlerFactory.Instance;
            var strings = new List<string>();
            var watch = new Stopwatch();
            // Lazy load the culture before we go ahead and use it in the test
            var dummyCulture = instance.CurrentUICulture;
            watch.Start();

            for(var i = 0; i < 1000; i++) {
                strings.Add(i.ToString("C", instance.CurrentUICulture));
            }

            watch.Stop();

            var ticks = watch.ElapsedTicks;

            // Expected less than 7 ms
            Assert.IsTrue(ticks < 70000, "ElapsedMilliseconds: " + watch.ElapsedMilliseconds.ToString());

        }

        [TestMethod]
        public void Convert1000IntsToCurrencyStrings_Using_nJupiterGlobalization_CurrentUICulture_Cached() {
            var instance = CultureHandlerFactory.Instance;
            // Here we 'cache' the culture and use later on in the test. This should be equal to using System.Threading.Thread.CurrentThread.CurrentUICulture.
            // However this isn't always how we could use it.
            var culture = instance.CurrentUICulture;
            var strings = new List<string>();
            var watch = new Stopwatch();
            watch.Start();

            for(var i = 0; i < 1000; i++) {
                strings.Add(i.ToString("C", culture));
            }

            watch.Stop();

            var ticks = watch.ElapsedTicks;

            // Less than a ms, should take 4800 ticks (approximately)
            Assert.IsTrue(ticks < 5000, "ElapsedMilliseconds: " + watch.ElapsedMilliseconds.ToString());

        }


        [TestMethod]
        public void Convert1000IntsToCurrencyStrings_Using_CurrentUICulture()
        {
            var strings = new List<string>();
            var watch = new Stopwatch();
            watch.Start();

            for (var i = 0; i < 1000; i++)
            {
                strings.Add(i.ToString("C", System.Threading.Thread.CurrentThread.CurrentUICulture));
            }

            watch.Stop();

            var ticks = watch.ElapsedTicks;

            // Less than a ms, should take 4800 ticks (approximately)
            Assert.IsTrue(ticks < 5000, "ElapsedMilliseconds: " + watch.ElapsedMilliseconds.ToString());

        }
    }
}
