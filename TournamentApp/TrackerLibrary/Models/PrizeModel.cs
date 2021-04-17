using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class PrizeModel
    {
        /// <summary>
        /// Unique Identifier for the prize.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Represent the place number of the team
        /// </summary>
        public int PlaceNumber { get; set; }
        /// <summary>
        /// Represent the place name of the team 
        /// </summary>
        public string  PlaceName { get; set; }
        /// <summary>
        /// Represent the amount for the winner
        /// </summary>
        public decimal PrizeAmount { get; set; }
        /// <summary>
        /// Represent the percentage of the winner
        /// </summary>
        public double PrizePercentage { get; set; }

        public PrizeModel()
        {

        }


        public PrizeModel(string placeName, string placeNumber, string prizeAmount, string prizePercentage)
        {
            PlaceName = placeName;

            int placeNumberValue = 0;
            int.TryParse(placeNumber, out placeNumberValue);
            PlaceNumber = placeNumberValue;

            decimal prizeAmountValue = 0;
            decimal.TryParse(prizeAmount, out prizeAmountValue);
            PrizeAmount = prizeAmountValue;

            double prizePercentageValue = 0;
            double.TryParse(prizePercentage, out prizePercentageValue);
            PrizePercentage = prizePercentageValue;
        }
    }
}