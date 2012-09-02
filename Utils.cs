/*
 * This file is part of EasyProgressDialog. EasyProgressDialog is free software: you can redistribute 
 * it and/or modify it under the terms of the GNU Lesser Public License as published by the Free 
 * Software Foundation, either version 3 of the License, or (at your option) any later version. 
 * Foobar is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even 
 * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser 
 * Public License for more details. You should have received a copy of the GNU Lesser Public License
 * along with EasyProgressDialog.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Text;

namespace KlerksSoft.EasyProgressDialog
{
    internal static class Utils
    {
        public static string GetApproxTimeSpanDescription(TimeSpan someTime)
        {
            string descOut = "Approx. ";

            if (someTime.TotalSeconds < 60)
                descOut += Math.Round(someTime.TotalSeconds).ToString() + " seconds";
            else if (someTime.TotalMinutes < 60)
                descOut += Math.Round(someTime.TotalMinutes, 1).ToString() + " minutes";
            else
                descOut += Math.Round(someTime.TotalHours, 1).ToString() + " hours";

            return descOut;
        }
    }
}
