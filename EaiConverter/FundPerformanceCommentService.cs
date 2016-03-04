namespace RoboAdvisor.Service
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class FundPerformanceCommentService
    {
        private const string portefeuille = "portefeuille";

        private static Dictionary<PerformanceOpinion, string> opinionAdjectiveMap = new Dictionary<PerformanceOpinion, string>
                                                                               {
                                                                                   { PerformanceOpinion.ReallyGood, "très bonne" },
                                                                                   { PerformanceOpinion.Good, "solide" },
                                                                                   { PerformanceOpinion.Neutral, "neutre" },
                                                                                   { PerformanceOpinion.Bad, "mauvaise" },
                                                                                   { PerformanceOpinion.ReallyBad, "très mauvaise" }
                                                                               };

        private Dictionary<PerformanceOpinion, Func<List<PerformanceOpinion>, double[], int, string[], string>> ContribCommentMap = new Dictionary<PerformanceOpinion, Func<List<PerformanceOpinion>, double[], int, string[], string>>
                                                                                                                                {
                                                                                                                                { PerformanceOpinion.ReallyGood, GetVeryGoodContribComment },
                                                                                                                                { PerformanceOpinion.Good, GetGoodContribComment },
                                                                                                                                { PerformanceOpinion.Neutral, GetNeutralContribComment },
                                                                                                                                { PerformanceOpinion.Bad, GetBadContribComment },
                                                                                                                                { PerformanceOpinion.ReallyBad, GetVeryBadContribComment}
                                                                                                                                };

        private Dictionary<PerformanceOpinion, Func<double, string>> perfCommentMap = new Dictionary<PerformanceOpinion, Func<double, string>>
                                                                                                                                {
                                                                                                                                { PerformanceOpinion.ReallyGood, (periodPerformance) => "La performance de votre " + portefeuille + " a été très bonne, atteignant " + periodPerformance + "% sur cette période." },
                                                                                                                                { PerformanceOpinion.Good, (periodPerformance) => "La performance de votre " + portefeuille + " a progressé de " + periodPerformance + "% sur cette période." },
                                                                                                                                { PerformanceOpinion.Neutral, (periodPerformance) => "La valeur de votre portefeuille n'pas évoluée sur cette période." },
                                                                                                                                { PerformanceOpinion.Bad, (periodPerformance) => "La performance de votre " + portefeuille + " est en baisse de " + periodPerformance + "% sur cette période." },
                                                                                                                                { PerformanceOpinion.ReallyBad, (periodPerformance) => "La performance de votre " + portefeuille + " est trés fortement en baisse, puisqu'il a perdu " + periodPerformance + "% sur cette période." }
                                                                                                                                };


        public string Get(double periodPerformance, string[] eventsOfThePeriod, SortedDictionary<double, string> contributions)
        {
            var comment = new StringBuilder();
            var ptfPerformanceOpinion = this.GetPerformanceOpinion(periodPerformance);
            comment.Append(this.GeneratePerformanceComment(periodPerformance, ptfPerformanceOpinion));
            comment.Append(" ");
            comment.Append(this.GeneratePeriodEvents(eventsOfThePeriod));
            comment.Append(" ");
            comment.Append(this.GenerateContributionComment(ptfPerformanceOpinion, contributions));
            return comment.ToString();
        }

        public PerformanceOpinion GetPerformanceOpinion(double periodPerformance)
        {
            if (periodPerformance > 5.0)
            {
                return PerformanceOpinion.ReallyGood;
            }
            else if (periodPerformance > 0 && periodPerformance <= 5)
            {
                return PerformanceOpinion.Good;
            }
            else if (periodPerformance == 0.0)
            {
                return PerformanceOpinion.Neutral;
            }
            else if (periodPerformance >= -5 && periodPerformance < 0)
            {
                return PerformanceOpinion.Bad;
            }
            else if (periodPerformance < -5)
            {
                return PerformanceOpinion.ReallyBad;
            }
            return PerformanceOpinion.Neutral;
        }

        public string GeneratePerformanceComment(double periodPerformance, PerformanceOpinion ptfPerformanceOpinion)
        {
            return (string)this.perfCommentMap[ptfPerformanceOpinion].DynamicInvoke(periodPerformance);
        }

        public string GeneratePeriodEvents(string[] eventsOfThePeriod)
        {
            if (eventsOfThePeriod.Length == 0)
            {
                return string.Empty;
            }

            if (eventsOfThePeriod.Length > 2)
            {
                //log.warning("on ne prend pas plus de 2 evenements !")
            }

            var result = "Période marquée par ";
            result = result + eventsOfThePeriod[0];
            if (eventsOfThePeriod.Length > 1)
            {
                result = result + " et par " + eventsOfThePeriod[1];
            }

            result = result + ".";
            return result;
        }

        public string GenerateContributionComment(PerformanceOpinion ptfPerformanceOpinion, SortedDictionary<double, string> contributions)
        {
            var lenght = contributions.Count;
            var keys = contributions.Keys;
            var keysArray = new double[keys.Count];
            keys.CopyTo(keysArray, 0);

            var values = contributions.Values;
            var valuesArray = new string[values.Count];
            values.CopyTo(valuesArray, 0);

            var underlyingPerformanceOpinion = this.GetPerformanceOpinions(keysArray);

            return (string)this.ContribCommentMap[ptfPerformanceOpinion].DynamicInvoke(underlyingPerformanceOpinion, keysArray, lenght, valuesArray);
        }

        private static string GetVeryGoodContribComment(List<PerformanceOpinion> underlyingPerformanceOpinion, double[] keysArray, int lenght, string[] valuesArray)
        {
            var result = "Les très bons résultats s'expliquent par une contribution "
                         + ToAdjective(underlyingPerformanceOpinion[lenght - 2]) + " de " + keysArray[lenght - 1] + "% des "
                         + valuesArray[lenght - 1] + " et de " + keysArray[lenght - 2] + "% des " + valuesArray[lenght - 2]
                         + ".";
            return result;
        }

        private static string GetGoodContribComment(List<PerformanceOpinion> underlyingPerformanceOpinion, double[] keysArray, int lenght, string[] valuesArray)
        {
            var result = "Les bons résultats s'expliquent par une contribution "
                         + ToAdjective(underlyingPerformanceOpinion[lenght - 2]) + " de " + keysArray[lenght - 1] + "% des "
                         + valuesArray[lenght - 1] + " et de " + keysArray[lenght - 2] + "% des " + valuesArray[lenght - 2]
                         + ".";
            return result;
        }

        private static string GetNeutralContribComment(List<PerformanceOpinion> underlyingPerformanceOpinion, double[] keysArray, int lenght, string[] valuesArray)
        {
            var result = "Les résultats s'expliquent par une contribution "
                         + ToAdjective(underlyingPerformanceOpinion[lenght - 2]) + " de " + keysArray[lenght - 1] + "% des "
                         + valuesArray[lenght - 1] + " et de " + keysArray[lenght - 2] + "% des " + valuesArray[lenght - 2]
                         + ".";
            return result;
        }

        private static string GetBadContribComment(List<PerformanceOpinion> underlyingPerformanceOpinion, double[] keysArray, int lenght, string[] valuesArray)
        {
            var result = "Les mauvais résultats s'expliquent par une contribution "
                         + ToAdjective(underlyingPerformanceOpinion[lenght - 2]) + " de " + keysArray[lenght - 1] + "% des "
                         + valuesArray[lenght - 1] + " et de " + keysArray[lenght - 2] + "% des " + valuesArray[lenght - 2]
                         + ".";
            return result;
        }

        private static string GetVeryBadContribComment(List<PerformanceOpinion> underlyingPerformanceOpinion, double[] keysArray, int lenght, string[] valuesArray)
        {
            var result = "Les très mauvais résultats s'expliquent par une contribution "
                         + ToAdjective(underlyingPerformanceOpinion[lenght - 2]) + " de " + keysArray[lenght - 1] + "% des "
                         + valuesArray[lenght - 1] + " et de " + keysArray[lenght - 2] + "% des " + valuesArray[lenght - 2]
                         + ".";
            return result;
        }

        private static string ToAdjective(PerformanceOpinion performanceOpinion)
        {
            return opinionAdjectiveMap[performanceOpinion];
        }

        private List<PerformanceOpinion> GetPerformanceOpinions(double[] valuesArray)
        {
            var performanceOpinions = new List<PerformanceOpinion>();
            foreach (var value in valuesArray)
            {
                performanceOpinions.Add(this.GetPerformanceOpinion(value));
            }

            return performanceOpinions;
        }
    }

    public enum PerformanceOpinion
    {
        ReallyGood,
        Good,
        Neutral,
        Bad,
        ReallyBad
    }
}

