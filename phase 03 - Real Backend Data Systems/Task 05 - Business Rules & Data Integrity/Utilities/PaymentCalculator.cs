using Task_05_Business_Rules_Data_Integrity.Entities;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.Utilities
{
    public class PaymentCalculator
    {
        public static (PaymentStatus Status, decimal TotalPaid) Calculate(Enrollment enrollment)
        {
            var trackPrice = enrollment.TrainingTrack?.Price ?? 0;
            var totalPaid = enrollment.Payments?
                .Where(p => p.PaymentStatus == PaymentStatus.Paid
                         || p.PaymentStatus == PaymentStatus.PartiallyPaid)
                .Sum(p => p.Amount) ?? 0;

            var status = totalPaid >= trackPrice && trackPrice > 0
                ? PaymentStatus.Paid
                : totalPaid > 0
                    ? PaymentStatus.PartiallyPaid
                    : PaymentStatus.Pending;

            return (status, totalPaid);
        }
    }
}
