namespace EaiConverter.Test.Builder
{
    using EaiConverter.Builder;
    using EaiConverter.Test.Utils;

    using NUnit.Framework;

    public class SubscriberBuilderTest
    {
        private SubscriberBuilder subscriberBuilder;

        [SetUp]
        public void SetUp()
        {
            this.subscriberBuilder = new SubscriberBuilder();
        }

        [Test]
        public void Should_generate_SubscriberInterface ()
        {
            var expected = @"namespace MyApp.Tools.EventSourcing
{
    using System;
    
    
    public interface ISubscriber
    {
        
        int WaitingTimeLimit
        {
            get;
        }
        
        bool IsStarted
        {
            get;
        }
        
        private event ResponseReceivedEventHandler ResponseReceived;
        
        void Start();
        
        void Stop();
    }
}
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.subscriberBuilder.GenerateClasses()[0]);
            Assert.AreEqual(expected, generatedCode);
        }
    }
}