using Microsoft.Extensions.Configuration;

namespace CrossPlatformSystemMonitorTest
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        [ExpectedException (typeof (FileNotFoundException))]
        public void ThrowsIfAppSettingsMissing()
        {
            var configBuilder = new ConfigurationBuilder()
                  .SetBasePath (Directory.GetCurrentDirectory())
                  .AddJsonFile ("nonexistent.json", optional: false, reloadOnChange: true);

            var config = configBuilder.Build();


        }
        [TestMethod]
        public void LoadsAppSettingsSuccessfully()
        {
            // Arrange: Use the real appsettings.json
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            // Act
            var config = configBuilder.Build();

            // Assert: Check a known value
            var interval = config["Monitoring:IntervalSeconds"];
            Assert.AreEqual("4", interval);
        }
    }
}
