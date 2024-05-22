using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
namespace Biljettshoppen
{
    public class Booking
    {
        public int BookingID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int EventID { get; set; }
        public List<int> SeatIDs { get; set; }
        public string PaymentMethod { get; set; }
        public string TicketID { get; set; } // Add the TicketID property
    }
    public class BookingManager : IBookingManager
{
    private List<Booking> bookings;
    private int nextBookingID;
    private string dataFile;
    private IEventManager eventManager;

    public BookingManager(string dataFile, IEventManager eventManager)
    {
        this.dataFile = dataFile;
        this.eventManager = eventManager;
        LoadData();
    }

        public Booking CreateBooking(string name, string email, int eventID, List<int> seatIDs, string paymentMethod)
        {
            Event selectedEvent = eventManager.GetEventById(eventID);

            if (selectedEvent != null)
            {
                if (seatIDs.Count > 5)
                {
                    Console.WriteLine("You can select up to 5 seats. Booking not created.");
                    return null;
                }

                bool allSeatsAvailable = seatIDs.All(seatID => selectedEvent.AvailableSeats.Contains(seatID));
                if (!allSeatsAvailable)
                {
                    Console.WriteLine("One or more of the selected seats are not available. Booking not created.");
                    return null;
                }

                try
                {
                    foreach (var seatID in seatIDs)
                    {
                        selectedEvent.AvailableSeats.Remove(seatID);
                        selectedEvent.UnavailableSeats.Add(seatID);
                    }

                    Booking newBooking = new Booking
                    {
                        BookingID = nextBookingID,
                        Name = name,
                        Email = email,
                        EventID = eventID,
                        SeatIDs = seatIDs,
                        PaymentMethod = paymentMethod
                    };

                    bookings.Add(newBooking);
                    nextBookingID++;

                    string ticketID = Guid.NewGuid().ToString();
                    newBooking.TicketID = ticketID;
                    SaveBookingToTextFile(newBooking);
                    SaveData();
                    eventManager.SaveData();
                    Console.WriteLine($"Booking created with ID: {newBooking.BookingID}");

                    return newBooking;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while creating the booking: " + ex.Message);
                    // You can log the exception or perform additional error handling here.
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Event not found. Booking not created.");
                return null;
            }
        }

        private void SaveBookingToTextFile(Booking booking)
        {
            // Create a unique text file name based on the TicketID
            string fileName = $"{booking.TicketID}.txt";
            
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Booking ID: " + booking.BookingID);
                writer.WriteLine("Ticket ID: " + booking.TicketID);
                writer.WriteLine("Name: " + booking.Name);
                writer.WriteLine("Email: " + booking.Email);
                writer.WriteLine("Event Name: " + eventManager.GetEventById(booking.EventID).EventName);
                writer.WriteLine("Event Venue: " + eventManager.GetEventById(booking.EventID).Venue);
                writer.WriteLine("Event ID: " + booking.EventID);
                writer.WriteLine("Seat IDs: " + string.Join(", ", booking.SeatIDs));
                writer.WriteLine("Payment Method: " + booking.PaymentMethod);
            }
        }

        public bool CancelBooking(int bookingID)
        {
            Booking bookingToCancel = bookings.Find(b => b.BookingID == bookingID);
            if (bookingToCancel != null)
            {
                Event eventToUpdate = eventManager.GetEventById(bookingToCancel.EventID);
                if (eventToUpdate != null)
                {
                    try
                    {
                        foreach (var seatID in bookingToCancel.SeatIDs)
                        {
                            eventToUpdate.AvailableSeats.Add(seatID);
                            eventToUpdate.UnavailableSeats.Remove(seatID);
                        }

                        bookings.Remove(bookingToCancel);
                        SaveData();

                        eventManager.SaveData(); // Save the changes to events.json

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred while canceling the booking: " + ex.Message);
                        // You can log the exception or perform additional error handling here.
                        return false;
                    }
                }
                return false;
            }
            return false;
        }
        
        public void ListBookings()
        {
            Console.WriteLine("List of Bookings:");
            try
            {
                foreach (var booking in bookings)
                {
                    Console.WriteLine($"Booking ID: {booking.BookingID}, Name: {booking.Name}, Email: {booking.Email}, Event ID: {booking.EventID}");
                    Console.WriteLine("Seat IDs: " + string.Join(", ", booking.SeatIDs));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while listing bookings: " + ex.Message);
                // You can log the exception or perform additional error handling here.
            }
        }

        private void LoadData()
        {
            if (File.Exists(dataFile))
            {
                string json = File.ReadAllText(dataFile);
                bookings = JsonSerializer.Deserialize<List<Booking>>(json);
                nextBookingID = bookings.Count > 0 ? bookings.Max(b => b.BookingID) + 1 : 1;
            }
            else
            {
                bookings = new List<Booking>();
                nextBookingID = 1;
            }
        }

        private void SaveData()
        {
            string json = JsonSerializer.Serialize(bookings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(dataFile, json);
        }
    }
}
