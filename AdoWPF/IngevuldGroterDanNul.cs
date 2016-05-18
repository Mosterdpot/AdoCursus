using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Globalization;

namespace AdoWPF
{
    public class IngevuldGroterDanNul : ValidationRule
    {

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            decimal getal;
            NumberStyles style = NumberStyles.Currency;
            // mag niet ingevuld zijn
            if (value == null || value.ToString() == string.Empty)
            {
                return ValidationResult.ValidResult;
            }
            if (!decimal.TryParse(value.ToString(), style, cultureInfo, out getal))
                if (!decimal.TryParse(value.ToString(), out getal))
                {
                    return new ValidationResult(false, "Waarde moet een getal zijn");
                }
            if (getal <= 0)
            {
                return new ValidationResult(false, "Getal moet groter zijn dan nul");
            }
            return ValidationResult.ValidResult;
        }

        //public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        //{
        //    decimal getal;
        //    if (value == null || value.ToString() == string.Empty)
        //    {
        //        return ValidationResult.ValidResult;
        //    }
        //    if (!decimal.TryParse(value.ToString(), out getal))
        //    {
        //        return new ValidationResult(false, "Waarde moet een getal zijn");
        //    }
        //    if (getal <= 0)
        //    {
        //        return new ValidationResult(false, "Getal moet groter zijn dan nul");
        //    }
        //    return ValidationResult.ValidResult;
        //}
    }
}
