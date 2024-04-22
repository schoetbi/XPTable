using XPTable.Renderers;

namespace UnitTest
{
    using System.Globalization;

    using NUnit.Framework;

    [TestFixture]
    public class NumberCellRendererTest
    {
        private static object[] TestData = new[]
                           {
                               new object[] { 1.0 / 3, "0.00", "0.33", CultureInfo.InvariantCulture},
                               new object[] { -1.0 / 3, "0.00", "-0,33", CultureInfo.GetCultureInfo("de")},
                               new object[] { 9.99999974737875E-05, "e", "9.99999974737875E-05", CultureInfo.InvariantCulture },
                               new object[] { 9.99999974737875E-05, "e", "9,99999974737875E-05", CultureInfo.GetCultureInfo("de") },
                               
                               // float
                               new object[] { 0.0001f, "e", "1.000000e-004", CultureInfo.InvariantCulture },
                               new object[] { 0.0001f, "0.0000", "0.0001", CultureInfo.InvariantCulture },

                               // double min max epsilon
                               new object[] { double.MinValue, "e", "-1.79769313486232E+308", CultureInfo.InvariantCulture },
                               new object[] { double.MaxValue, "e", "1.79769313486232E+308", CultureInfo.InvariantCulture },
                               new object[] { double.Epsilon, "e", "4.94065645841247E-324", CultureInfo.InvariantCulture },

                               // wrong format
                               new object[] { double.Epsilon, "yyy", "4.94065645841247E-324", CultureInfo.InvariantCulture },

                               // a nonnumber object
                               new object[] { "nonnumber", "yyy", "nonnumber", CultureInfo.InvariantCulture },
                               
                               // (u)int
                               new object[] { 34, "0", "34", CultureInfo.InvariantCulture },
                               new object[] { -34, "0", "-34", CultureInfo.InvariantCulture },
                               new object[] { 34u, "0", "34", CultureInfo.InvariantCulture },
                               
                               // (u)short
                               new object[] { ((short)34), "0", "34", CultureInfo.InvariantCulture },
                               new object[] { ((short)-34), "0", "-34", CultureInfo.InvariantCulture },
                               new object[] { ((ushort)34), "0", "34", CultureInfo.InvariantCulture },

                               // (s)byte
                               new object[] { ((sbyte)34), "0", "34", CultureInfo.InvariantCulture },
                               new object[] { ((sbyte)-34), "0", "-34", CultureInfo.InvariantCulture },
                               new object[] { ((byte)34), "0", "34", CultureInfo.InvariantCulture },
                           };



        [Test]
        [TestCaseSource(nameof(TestData))]
        public void CheckNumberRenderer(object data, string format, string expected, CultureInfo culture)
        {
            var txt = NumberCellRenderer.RenderText(data, format, culture);
            Assert.AreEqual(expected, txt);
        }
    }
}
