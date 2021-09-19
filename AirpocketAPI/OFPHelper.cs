using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirpocketAPI
{
    public class OFPHelper
    {
        public static List<string> GetLineParts(string line,bool keepStart=false)
        {
            var prts = line.Split(' ').Where(q => !string.IsNullOrEmpty(q)).ToList();
            if (keepStart)
                return prts;
            else
                return prts.Skip(0).ToList();

        }
        public static string GetLineStartsWith(List<string> source, List<string> nospace, string key)
        {
            key = key.Replace(" ", "").ToLower();
            var index = nospace.IndexOf(nospace.Where(q => q.ToLower().StartsWith(key)).First());
            return source[index];
        }

        public static List<FuelLine> GetFuelParts(List<string> lines, List<string> linesNoSpace)
        {
            var _fuelKey = ("FUEL  TIME  DIST ARRIVE TAKEOFF  LNDG   PLD   OPNLWT").Replace(" ", "").ToLower();
            var _fuelIndex = linesNoSpace.IndexOf(linesNoSpace.Where(q => q.ToLower() == _fuelKey).First());
            var _fuelIndexLast = linesNoSpace.IndexOf(linesNoSpace.Where(q => q.ToLower().StartsWith( ("HEIGHT CHANGE").Replace(" ", "").ToLower())).First());
            var fuelLines = lines.Skip(_fuelIndex + 1).Take(_fuelIndexLast - _fuelIndex - 1).ToList();
            List<FuelLine> result = new List<FuelLine>();

            var dstLn = GetLineStartsWith(lines, linesNoSpace, "DEST");
            var dstParts = GetLineParts(dstLn);
            //                           FUEL       TIME     DIST   ARRIVE    TAKEOFF    LNDG       PLD      OPNLWT
            //           0DEST (1)OIMM (2)010362 (3)01.16 (4)0502 (5)18:21Z  (6)134797 (7)124435 (8)026500 (9)085418
            result.Add(new FuelLine()
            {
                Title = "DEST",
                ICAO = dstParts[1],
                FUEL = dstParts[2],
                TIME = dstParts[3],
                DIST = dstParts[4],
                ARRIVE = dstParts[5],
                TAKEOFF = dstParts[6],
                LNDG = dstParts[7],
                PLD = dstParts[8],
                OPNLWT = dstParts[9],
                Line=dstLn,

            });
            var altnLn = GetLineStartsWith(lines, linesNoSpace, "ALTN");
            var altnParts = GetLineParts(altnLn);
            result.Add(new FuelLine()
            {
                Title = "ALTN",
                ICAO = altnParts[1],
                FUEL = altnParts[2],
                TIME = altnParts[3],
                DIST = altnParts[4],
                Line=altnLn,
                 

            });
            var hldLn = GetLineStartsWith(lines, linesNoSpace, "HLD");
            var hldParts = GetLineParts(hldLn);
            result.Add(new FuelLine()
            {
                Title = "HLD",
                FUEL = hldParts[1],
                TIME = hldParts[2],
                Line=hldLn,
            });
            var ln = GetLineStartsWith(lines, linesNoSpace, "Cont 05%");
            var lnprts = GetLineParts(ln);
            result.Add(new FuelLine()
            {
                Title = "CONT05",
                FUEL = lnprts[2],
                TIME = lnprts[3],
                Line=ln,
            });
            ln = GetLineStartsWith(lines, linesNoSpace, "TXY");
            lnprts = GetLineParts(ln);
            result.Add(new FuelLine()
            {
                Title = "TXY",
                FUEL = lnprts[1],
               
                Line = ln,
            });
            ln = GetLineStartsWith(lines, linesNoSpace, "REQUIRED");
            lnprts = GetLineParts(ln);
            result.Add(new FuelLine()
            {
                Title = "REQUIRED",
                FUEL = lnprts[1],
                TIME = lnprts[2],
                Line = ln,
            });
            ln = GetLineStartsWith(lines, linesNoSpace, "ADDITION");
            lnprts = GetLineParts(ln);
            result.Add(new FuelLine()
            {
                Title = "ADDITION",
                FUEL = lnprts[1],
                TIME = lnprts[2],
                Line = ln,
            });
            ln = GetLineStartsWith(lines, linesNoSpace, "XTR");
            lnprts = GetLineParts(ln);
            result.Add(new FuelLine()
            {
                Title = "XTR",
                FUEL = lnprts[1],
                TIME = lnprts[2],
                Line = ln,
            });
            ln = GetLineStartsWith(lines, linesNoSpace, "TOTAL");
            lnprts = GetLineParts(ln);
            result.Add(new FuelLine()
            {
                Title = "TOTAL",
                FUEL = lnprts[1],
                TIME = lnprts[2],
                Remark=string.Join(" ", lnprts.Skip(3).ToList()),
                Line = ln,
            });

            return result;
        }
    }


    public class FuelLine
    {
        // public bool IsALT { get; set; }
        public string Title { get; set; }
        public string ICAO { get; set; }
        public string FUEL { get; set; }
        public string DIST { get; set; }
        public string ARRIVE { get; set; }
        public string TAKEOFF { get; set; }
        public string LNDG { get; set; }
        public string PLD { get; set; }
        public string OPNLWT { get; set; }
        public string TIME { get; set; }
        public string Remark { get; set; }
        public string Line { get; set; }
    }
}