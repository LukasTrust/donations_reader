using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DonationsProject.Classes.Objects
{
    public class Donation
    {
        #region Statics

        public static List<Donation> Donations { get; set; } = new List<Donation>();

        #endregion

        #region Properties  

        public string Party { get; set; }
        public double Amount { get; set; }
        public string Donor { get; set; }
        public DateTime DonationDate { get; set; }
        public DateTime ReportDate { get; set; }

        #endregion

        #region Create from DB

        public static async Task CreateDonationFromDB(object[] data)
        {
            Donation donation = new Donation();
            await Task.Run(() =>
            {
                donation.DonationDate = Convert.ToDateTime(data[0]);
                donation.ReportDate = Convert.ToDateTime(data[1]);
                donation.Donor = data[2].ToString();
                donation.Party = data[3].ToString();
                donation.Amount = Convert.ToDouble(data[4]);

            });
            Donations.Add(donation);
        }

        #endregion

        #region Create from Website

        public static async Task CreateDonationFromWebsite(Match match)
        {
            Donation donation = new Donation();
            await Task.Run(() =>
            {
                int beginnP = 0;
                int endP = 0;
                string forConversion = "";

                forConversion = match.Groups[1].Value;
                beginnP = forConversion.IndexOf('>');
                endP = forConversion.IndexOf('<', beginnP);
                forConversion = forConversion.Substring(beginnP + 1, endP - beginnP - 1);
                if (forConversion.Contains("&nbsp;"))
                {
                    forConversion = forConversion.Replace("&nbsp;", "");
                }
                donation.Party = forConversion;

                beginnP = match.Groups[2].Value.IndexOf('>');
                endP = match.Groups[2].Value.IndexOf('<', beginnP);
                forConversion = match.Groups[2].Value.Substring(beginnP + 1, endP - beginnP - 1);

                forConversion = forConversion.Replace("&nbsp;", "");
                forConversion = forConversion.Replace("Euro", "");
                forConversion = forConversion.Replace(" ", "");

                donation.Amount = Convert.ToDouble(forConversion);

                beginnP = match.Groups[3].Value.IndexOf('>');
                endP = match.Groups[3].Value.IndexOf('<', beginnP);
                donation.Donor = match.Groups[3].Value.Substring(beginnP + 1, endP - beginnP - 1);

                beginnP = match.Groups[5].Value.IndexOf('>');
                endP = match.Groups[5].Value.IndexOf('<', beginnP);
                forConversion = match.Groups[5].Value.Substring(beginnP + 1, endP - beginnP - 1);

                forConversion = Regex.Replace(forConversion, "[a-zA-Z]", "");
                if (forConversion.Length > 10)
                {
                    forConversion = forConversion.Remove(10, forConversion.Length - 10);
                }

                donation.ReportDate = Convert.ToDateTime(forConversion);

                beginnP = match.Groups[4].Value.IndexOf('>');
                endP = match.Groups[4].Value.IndexOf('<', beginnP);
                forConversion = match.Groups[4].Value.Substring(beginnP + 1, endP - beginnP - 1);

                if (forConversion.Length == 6)
                {
                    forConversion = forConversion + donation.ReportDate.Year;
                }
                forConversion = forConversion.Replace("/", "");
                forConversion = Regex.Replace(forConversion, "[a-zA-Z]", "");

                donation.DonationDate = Convert.ToDateTime(forConversion);
            });

            Donations.Add(donation);
        }

        public static async Task CreateDonationFromWebsite2009(string content)
        {
            Donation donation = new Donation();
            await Task.Run(() =>
            {
                if (content.Length < 30)
                {
                    return;
                }
                string[] splitContent = content.Split(' ');
                donation.Party = splitContent[0];
                int ammoutIndex = -1;

                try
                {
                    double convertTest = Convert.ToDouble(splitContent[2]);
                    string ammout = splitContent[1] + splitContent[2];
                    donation.Amount = Convert.ToDouble(ammout);
                    ammoutIndex = 1;
                }
                catch
                {
                    try
                    {
                        donation.Amount = Convert.ToDouble(splitContent[1]);
                        ammoutIndex = 1;
                    }
                    catch
                    {
                        string ammout = splitContent[splitContent.Length - 2] + splitContent[splitContent.Length - 1];
                        try
                        {
                            donation.Amount = Convert.ToDouble(ammout);
                            if (donation.Amount > 10000000)
                            {
                                return;
                            }
                            ammoutIndex = splitContent.Length - 2;
                        }
                        catch
                        {
                            return;
                        }
                    }
                }
                int receiptDateIndex = -1;
                for (int i = 0; i < splitContent.Length; i++)
                {
                    try
                    {
                        string cleanString = splitContent[i];
                        cleanString = cleanString.Replace("per", "");
                        DateTime convertTest = Convert.ToDateTime(cleanString);
                        donation.DonationDate = convertTest;
                        receiptDateIndex = i;
                        break;
                    }
                    catch
                    {

                    }
                }
                if (receiptDateIndex != -1)
                {
                    try
                    {
                        donation.ReportDate = Convert.ToDateTime(splitContent[receiptDateIndex + 1]);
                    }
                    catch
                    {
                        donation.ReportDate = donation.DonationDate;
                    }
                }

                string donor = "";
                for (int i = 0 + 1; i < splitContent.Length; i++)
                {
                    if (!double.TryParse(splitContent[i], out double test))
                    {
                        donor += splitContent[i] + " ";
                    }
                }
                donor = donor.Replace("\n", "");
                donor = donor.TrimEnd(' ');
                donor = donor.Replace("- ", "");
                donation.Donor = donor;
            });
            Donations.Add(donation);
        }
        public static async Task CreateDonationFromWebsite2015(Match match)
        {
            Donation donation = new Donation();
            await Task.Run(() =>
            {
                string forConversion = "";
                int beginnP = 0;
                int endP = 0;
                forConversion = match.Groups[1].Value;
                if (forConversion.Contains("&nbsp;"))
                {
                    forConversion = forConversion.Replace("&nbsp;", "");
                }
                donation.Party = forConversion;

                forConversion = match.Groups[2].Value;
                if (forConversion.Contains("Korrigierter"))
                {
                    beginnP = forConversion.IndexOf('>');
                    endP = forConversion.IndexOf('<', beginnP);
                    forConversion = forConversion.Substring(beginnP + 1, endP - beginnP - 1);
                }
                else
                if (forConversion.Contains('<'))
                {
                    beginnP = forConversion.IndexOf('<');
                    forConversion = forConversion.Remove(beginnP, forConversion.Length - beginnP);
                }
                donation.Amount = Convert.ToDouble(forConversion);

                forConversion = match.Groups[3].Value;
                beginnP = forConversion.IndexOf('<');
                forConversion = forConversion.Remove(beginnP, forConversion.Length - beginnP);
                donation.Donor = forConversion;

                forConversion = match.Groups[4].Value;
                if (forConversion.Contains('/') && !forConversion.Contains('p'))
                {
                    beginnP = forConversion.IndexOf('/');
                    forConversion = forConversion.Remove(0, beginnP + 1);
                }
                else
                if (forConversion.Length > 10 && forConversion.Contains('p'))
                {
                    forConversion = forConversion.Remove(10, forConversion.Length - 10);
                }
                else
                if (forConversion.Contains('-'))
                {
                    beginnP = forConversion.IndexOf('-');
                    forConversion = forConversion.Substring(beginnP + 1, forConversion.Length - beginnP - 1);
                }
                if (match.Groups[4].Value.Contains("per"))
                {
                    forConversion = match.Groups[4].Value.Remove(0, 4);
                }
                donation.DonationDate = Convert.ToDateTime(forConversion);

                forConversion = match.Groups[5].Value;
                if (forConversion.Contains('<'))
                {
                    beginnP = forConversion.IndexOf('<');
                    forConversion = forConversion.Remove(beginnP, forConversion.Length - beginnP);
                }
                try
                {
                    donation.ReportDate = Convert.ToDateTime(forConversion);
                }
                catch
                {
                    donation.ReportDate = Convert.ToDateTime("12.02.2010");
                }
            });
            Donations.Add(donation);
        }

        #endregion

        #region Clean up data

        public static async Task CleanUpData()
        {
            List<Donation> donationsToChange = new List<Donation>();

            await Task.Run(() =>
            {

                donationsToChange = Donations.Where(x => x.Party == null).ToList();
                donationsToChange.AddRange(Donations.Where(x => x.Amount == 0).ToList());
                donationsToChange.AddRange(Donations.Where(x => x.Amount > 100000000).ToList());
                donationsToChange.AddRange(Donations.Where(x => x.Donor == null).ToList());
                donationsToChange.AddRange(Donations.Where(x => x.ReportDate.Year < 2002).ToList());
                donationsToChange.AddRange(Donations.Where(x => x.ReportDate.Year > 2023).ToList());

                foreach (Donation donation in donationsToChange)
                {
                    Donations.Remove(donation);
                }

                donationsToChange = Donations.Where(x => x.Donor.Contains("Coesfeldweg")).ToList();
                donationsToChange.AddRange(Donations.Where(x => x.Donor.Contains("Epplestraße")).ToList());
                donationsToChange.AddRange(Donations.Where(x => x.Donor.Contains("Max-Joseph-Straße")).ToList());
                donationsToChange.AddRange(Donations.Where(x => x.Donor.Contains("Mühlenberger Weg Hamburg")).ToList());
                foreach (Donation donation in donationsToChange)
                {
                    Donations.Remove(donation);
                }

                donationsToChange = Donations.Where(x => x.Party.ToLower().Contains("bündnis")).ToList();
                donationsToChange.ForEach(x => x.Party = "Bündnis 90/Die Grünen");
                donationsToChange = Donations.Where(x => x.Party.ToLower().Contains("grüne")).ToList();
                donationsToChange.ForEach(x => x.Party = "Bündnis 90/Die Grünen");

                donationsToChange = Donations.Where(x => x.Party.ToLower().Contains("linke")).ToList();
                donationsToChange.ForEach(x => x.Party = "Die Linke");

                donationsToChange = Donations.Where(x => x.Party.ToLower().Contains("fdp")).ToList();
                donationsToChange.ForEach(x => x.Party = "FDP");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("&amp;")).ToList();
                donationsToChange.ForEach(x => x.Donor = x.Donor.Replace("&amp;", " "));

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("&nbsp;")).ToList();
                donationsToChange.ForEach(x => x.Donor = x.Donor.Replace("&nbsp;", " "));

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("allianz ag")).ToList();
                donationsToChange.AddRange(Donations.Where(x => x.Donor.ToLower().Contains("allianz deutschland ag")).ToList());
                donationsToChange.AddRange(Donations.Where(x => x.Donor.ToLower().Contains("allianz v")).ToList());
                donationsToChange.ForEach(x => x.Donor = "Allianz AG");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("allianz se")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Allianz SE");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("altana ag")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Altana AG");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("bankhaus sal.")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Bankhaus Sal. Oppenheim Jr. & Cie KGaA");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("bayerische motoren")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Bayerische Motoren Werke AG");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("berenberg bank")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Berenberg Bank Joh. Berenberg, Gossler & Co.");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("bmw ag")).ToList();
                donationsToChange.ForEach(x => x.Donor = "BMW AG");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("commerzbank ag")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Commerzbank AG");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("daiml")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Daimler AG");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("deutsche bank")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Deutsche Bank AG");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("deutsche vermögensberatung")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Deutsche Vermögensberatung AG");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("dr. rath education")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Dr. Rath Education Services B. V.");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("dr. rath health")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Dr. Rath Health Programs B. V.");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("e.on ag")).ToList();
                donationsToChange.ForEach(x => x.Donor = "E.ON AG");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("evonik")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Evonik Industries AG");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("industriebeteiligungen gmbh")).ToList();
                donationsToChange.ForEach(x => x.Donor = "R + W Industriebeteiligungen GmbH");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("frau johanna quandt")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Johanna Quandt");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("frau susanne klatten")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Susanne Klatten");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("herr christoph")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Christoph Kahl");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("michael may")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Michael May");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("hans-joachim langmann")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Prof. Dr. Hans-Joachim Langmann");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("stefan quandt")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Stefan Quandt");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("märkischer arbeitgeberverband")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Märkischer Arbeitgeberverband e. V.");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("matthias rath limited")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Matthias Rath Limited");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("metall nrw")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Metall NRW - Verband der Metall- und Elektro-Industrie Nordrhein-Westfalen e. V.");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("petuelring")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Petuelring");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("hermann schnabel")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Prof. Dr. h. c. Hermann Schnabel");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("näder")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Prof. Otto Max Hans-Georg Näder");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("rag aktiengesellschaft")).ToList();
                donationsToChange.ForEach(x => x.Donor = "RAG Aktiengesellschaft");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("reiner sauer")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Reiner Sauer");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("sixt gmbh")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Sixt GmbH & Co. Autovermietung KG");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("südwestmetall")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Südwestmetall - Verband der Metall- und Elektroindustrie Baden-Württemberg e. V.");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("sydslesvigudvalget")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Sydslesvigudvalget/ Kulturministeriet, Kulturstyrelsen");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().StartsWith("herr")).ToList();
                donationsToChange.ForEach(x => x.Donor = x.Donor.Replace("Herr", ""));

                donationsToChange = Donations.Where(x => x.Donor.ToLower().StartsWith("frau")).ToList();
                donationsToChange.ForEach(x => x.Donor = x.Donor.Replace("Frau", ""));

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("taunusanlage")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Taunusanlage");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("trumpf")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Trumpf GmbH & Co. KG");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("verband der bayerischen")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Verband der Bayerischen Metall- und Elektroindustrie e. V.");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("verband der chemischen")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Verband der Chemischen Industrie e. V.");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().StartsWith("verband der metall")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Verband der Metall- und Elektroindustrie Nordrhein-Westfalen e. V.");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("verein der bayerischen chemischen")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Verein der Bayerischen Chemischen Industrie e. V.");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("e.v.")).ToList();
                donationsToChange.ForEach(x => x.Donor = x.Donor.Replace("e.V.", "e. V."));

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("gröner")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Gröner Family Office Gmbh");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("abels & grey")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Abels & Grey");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("aida media ltd.")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Aida Media Ltd.");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("ann kathrin linsenhoff")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Ann Kathrin Linsenhoff");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("bei den angezeigten spenden handelt es sich sämtlich um kostenlose fahrzeugüberlassungen")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Bei den angezeigten Spenden handelt es sich sämtlich um kostenlose Fahrzeugüberlassungen");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("capital lease gmbh")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Capital Lease GmbH");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("clou container")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Clou Container Leasing GmbH");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("dr. cornelius boersch")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Dr. Cornelius Boersch");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("e. h. martin")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Dr. Ing. E. h. Martin Herrenknecht");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("dr. karin fischer")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Dr. Karin Fischer");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("eurolottoclub ag")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Eurolottoclub AG");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("kurt fordan")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Kurt Fordan");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("media service")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Media Service");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("mercator verwaltung gmbh")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Mercator Verwaltung GmbH");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("sal. oppenheim jr. & cie. kgaa")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Sal. Oppenheim jr. & Cie. KGaA");

                donationsToChange = Donations.Where(x => x.Donor.ToLower().Contains("schoeller holdings ltd.")).ToList();
                donationsToChange.ForEach(x => x.Donor = "Schoeller Holdings Ltd.");

                Donations.ForEach(x => x.Donor = RemoveExcessSpaces(x.Donor));
            });
        }

        public static string RemoveExcessSpaces(string input)
        {
            // Teile den Eingabe-String in Wörter auf, trennend durch Leerzeichen
            string[] words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Füge die Wörter wieder zusammen, trennend durch ein Leerzeichen
            string cleanedString = string.Join(" ", words);

            return cleanedString;
        }

        #endregion
    }
}
