using System.Globalization;
using System.Text.RegularExpressions;

namespace budget_api.Models.CSV.Strategies
{
    public static class DescriptionCleaner
    {
        // DEBIT_CARD: extract merchant name from domain (e.g. REMITLY.COM → Remitly, NETFLIX.COM → Netflix)
        private static readonly Regex DebitCardDomain =
            new(@"\b([A-Z0-9][A-Z0-9\-]+)\.(COM|NET|ORG|IO|CO)\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // DEBIT_CARD: strip trailing date MM/YY
        private static readonly Regex DebitCardTrailingDate =
            new(@"\s+\d{2}/\d{2}$", RegexOptions.Compiled);

        // DEBIT_CARD: strip trailing 2-letter state abbreviation
        private static readonly Regex DebitCardTrailingState =
            new(@"\s+[A-Z]{2}$", RegexOptions.Compiled);

        // DEBIT_CARD: strip trailing phone number
        private static readonly Regex DebitCardTrailingPhone =
            new(@"\s+\d{3}[.\-]\d{3}[.\-]\d{4}$", RegexOptions.Compiled);

        // DEBIT_CARD: strip trailing *CODE or #CODE or space-separated code
        private static readonly Regex DebitCardTrailingCode =
            new(@"[\s*#]\S+$", RegexOptions.Compiled);

        // ACH: extract "Orig CO Name:" value, stop at next field label
        private static readonly Regex AchOrigCoName =
            new(@"Orig CO Name:([^\n]+?)(?:\s+Orig ID:|\s+Desc:|$)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // ACH fallback: strip "PPD ID: ...", "WEB ID: ...", "CCD ID: ..." metadata
        private static readonly Regex AchIdSuffix =
            new(@"\s+(PPD|WEB|CCD|TEL)\s+ID:.*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // ACH fallback: strip leading numeric routing/batch codes (e.g. "193270 ")
        private static readonly Regex AchLeadingDigits =
            new(@"^\d+\s+", RegexOptions.Compiled);

        // ACH fallback: strip trailing alphanumeric transaction codes (6+ chars, must contain a digit)
        // Requiring a digit prevents stripping legitimate ALL-CAPS words like "PAYROLL"
        private static readonly Regex AchTrailingCode =
            new(@"\s+(?=[A-Z0-9]*\d)[A-Z0-9]{6,}$", RegexOptions.Compiled);

        // ACCT_XFER: "Online Transfer to/from chk ...1234"
        private static readonly Regex AcctXferTo =
            new(@"Online Transfer to chk\s*(\.{3}\d+)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex AcctXferFrom =
            new(@"Online Transfer from chk\s*(\.{3}\d+)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // QUICKPAY: strip trailing transaction ID (long digit sequence at end)
        private static readonly Regex QuickPayTrailingId =
            new(@"\s+\d{8,}$", RegexOptions.Compiled);

        // LOAN_PMT: strip trailing date MM/DD
        private static readonly Regex LoanPmtTrailingDate =
            new(@"\s+\d{2}/\d{2}$", RegexOptions.Compiled);

        // SAPPHIRE: strip trailing *CODE noise
        private static readonly Regex SapphireNoise =
            new(@"\*\S+$", RegexOptions.Compiled);

        public static string CleanChecking(string? raw, string? bankTxnType)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

            return (bankTxnType?.ToUpperInvariant()) switch
            {
                "DEBIT_CARD"                => CleanDebitCard(raw),
                "ACH_DEBIT" or "ACH_CREDIT" => CleanAch(raw),
                "ACCT_XFER"                 => CleanAcctXfer(raw),
                "QUICKPAY_DEBIT"            => CleanQuickPay(raw),
                "QUICKPAY_CREDIT"           => CleanQuickPay(raw),
                "LOAN_PMT"                  => CleanLoanPmt(raw),
                "MISC_DEBIT" or "MISC_CREDIT" => CleanMisc(raw),
                _                           => ToTitleCase(raw),
            };
        }

        public static string CleanSapphire(string? description)
        {
            if (string.IsNullOrWhiteSpace(description)) return string.Empty;

            var cleaned = SapphireNoise.Replace(description.Trim(), string.Empty);
            return ToTitleCase(cleaned);
        }

        private static string CleanDebitCard(string raw)
        {
            // If a domain name is present, use the domain stem as the merchant name
            // e.g. "RMTLY* F0EDF REMITLY.COM WA 01/30" → "Remitly"
            var domainMatch = DebitCardDomain.Match(raw);
            if (domainMatch.Success)
                return ToTitleCase(domainMatch.Groups[1].Value);

            // Otherwise strip noise from right to left: date → state → phone → city → code
            var result = DebitCardTrailingDate.Replace(raw.Trim(), string.Empty);
            result = DebitCardTrailingState.Replace(result.Trim(), string.Empty);
            result = DebitCardTrailingPhone.Replace(result.Trim(), string.Empty);
            result = DebitCardTrailingCode.Replace(result.Trim(), string.Empty);
            // Apply once more to catch cases with both a phone and a trailing code
            result = DebitCardTrailingCode.Replace(result.Trim(), string.Empty);
            return ToTitleCase(result.Trim());
        }

        private static string CleanAch(string raw)
        {
            // Try to extract structured "Orig CO Name:" field
            var match = AchOrigCoName.Match(raw);
            if (match.Success)
                return ToTitleCase(match.Groups[1].Value.Trim());

            // Fallback: strip ACH metadata to surface the readable company name
            var result = AchIdSuffix.Replace(raw.Trim(), string.Empty);   // strip "PPD ID: ..."
            result = AchLeadingDigits.Replace(result.Trim(), string.Empty); // strip leading "193270 "
            result = AchTrailingCode.Replace(result.Trim(), string.Empty);  // strip trailing codes
            return ToTitleCase(result.Trim());
        }

        private static string CleanAcctXfer(string raw)
        {
            var toMatch = AcctXferTo.Match(raw);
            if (toMatch.Success)
                return $"Transfer to Checking {toMatch.Groups[1].Value}";

            var fromMatch = AcctXferFrom.Match(raw);
            if (fromMatch.Success)
                return $"Transfer from Checking {fromMatch.Groups[1].Value}";

            return ToTitleCase(raw);
        }

        private static string CleanQuickPay(string raw)
        {
            // Keep "Zelle Payment To/From" prefix — just strip trailing transaction ID
            // e.g. "Zelle payment to John Rick Santillan 27788200534" → "Zelle Payment To John Rick Santillan"
            var result = QuickPayTrailingId.Replace(raw.Trim(), string.Empty);
            return ToTitleCase(result.Trim());
        }

        private static string CleanLoanPmt(string raw)
        {
            // Keep "Payment to" prefix — just strip trailing date
            // e.g. "Payment to Chase card ending in 1850 01/28" → "Payment To Chase Card Ending In 1850"
            var result = LoanPmtTrailingDate.Replace(raw.Trim(), string.Empty);
            return ToTitleCase(result.Trim());
        }

        private static string CleanMisc(string raw)
        {
            // Strip WEB ID / transaction metadata, then title-case
            var result = AchIdSuffix.Replace(raw.Trim(), string.Empty);
            result = AchTrailingCode.Replace(result.Trim(), string.Empty);
            return ToTitleCase(result.Trim());
        }

        private static string ToTitleCase(string input) =>
            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }
}
