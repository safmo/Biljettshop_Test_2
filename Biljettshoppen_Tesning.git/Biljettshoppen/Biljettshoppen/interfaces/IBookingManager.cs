namespace Biljettshoppen
{
    public interface IBookingManager
    {
        Booking CreateBooking(string name, string email, int eventID, List<int> seatIDs, string paymentMethod);
        bool CancelBooking(int bookingID);
        void ListBookings();
    }
}