namespace RoboAdvisor.Service.Test
{
    using System.Collections.Generic;

    using NUnit.Framework;

    [TestFixture]
    public class FundPerformanceCommentServiceTest
    {
        private FundPerformanceCommentService service;

        [SetUp]
        public void SetUp()
        {
            this.service = new FundPerformanceCommentService();
        }

        [Test]
        public void Should_return_a_good_opinion_when_performance_is_4pct()
        {
            var actual = this.service.GetPerformanceOpinion(4.3);

            Assert.AreEqual(PerformanceOpinion.Good, actual);
        }


        [Test]
        public void Should_return_good_comment_when_perform_is_4pct()
        {
            var expected = "La performance de votre portefeuille a progressé de 4.3% sur cette période. Période marquée par le carnaval de Dunkerque et par la foire au jambon. Les bons résultats s'expliquent par une contribution solide de 1.2% des actions francaises et de 0.4% des obligations europeennes.";

            var actual = this.service.Get(
                4.3,
                new string[] { "le carnaval de Dunkerque", "la foire au jambon" },
                new SortedDictionary<double, string>
                {
                    { 1.2, "actions francaises" },
                    { 0.4, "obligations europeennes" }});

            Assert.AreEqual(expected, actual);
        }

    }
}
