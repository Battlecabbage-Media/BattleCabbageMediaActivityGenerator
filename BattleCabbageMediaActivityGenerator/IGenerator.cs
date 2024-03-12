using BattleCabbageMediaActivityGenerator.Models;

namespace BattleCabbageMediaActivityGenerator
{
    internal interface IGenerator
    {
        bool CheckChances(int instance_rate, int total_rate);
        Task CreateRentals(decimal lateFee, decimal rentalFee, DateTime? runDate = null);
        Task CreateReturns(decimal lateFee, DateTime? runDate = null);
        Task GenerateAsync(DateTime? runDate = null);
        Task GeneratePastActivity(DateTime startDate, DateTime endDate);
        Task GenerateCurrentActivity();
        Movie GetRandomMovie();
        void SetGenerationSettings(DateTime? runDate = null, bool initialization = false);
    }
}