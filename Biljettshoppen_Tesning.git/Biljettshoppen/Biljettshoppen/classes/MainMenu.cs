using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
namespace Biljettshoppen
{
    public class MainMenu
    {
        private IBookingManager bookingManager;
        private IEventManager eventManager;

        public MainMenu()
        {
            eventManager = new EventManager("events.json");
            bookingManager = new BookingManager("bookings.json", (EventManager)eventManager); // Cast to EventManager
        }


        public void Run()
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();

                Console.WriteLine("Welcome! Are you a User or Admin? ");
                Console.WriteLine("1. User");
                Console.WriteLine("2. Admin");
                Console.WriteLine("3. Exit");
                Console.Write("Select an option: ");
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    try
                    {
                        switch (choice)
                        {
                            case 1:
                                UserMenu();
                                break;

                            case 2:
                                if (AdminLogin())
                                {
                                    AdminMenu();
                                }
                                break;

                            case 3:
                                Console.WriteLine("Exiting Program.");
                                isRunning = false;
                                break;
                                }
                    }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }
    }
    
        private bool AdminLogin()
        {
            Console.Clear();
            Console.Write("Enter Admin Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Admin Password: ");
            string password = Console.ReadLine();
            if (username == "admin" && password == "admin")
            {
                Console.WriteLine("Admin login successful.");
                Console.WriteLine("Press Enter to continue.");
                Console.ReadLine();
                return true;
            }
            else
            {
                Console.WriteLine("Invalid admin credentials. Press Enter to return to the main menu.");
                Console.ReadLine();
                return false;
            }
        }

        public void UserMenu()
        {
            bool isUserMenuRunning = true;
            while (isUserMenuRunning)
            {
                Console.Clear();

                Console.WriteLine("User Menu:");
                Console.WriteLine("1. Book a Ticket");
                Console.WriteLine("2. Cancel Booking");
                Console.WriteLine("3. Return to Main Menu");
                Console.Write("Select an option: ");

                if (int.TryParse(Console.ReadLine(), out int userChoice))
                {
                    try
                    {
                        switch (userChoice)
                        {
                            case 1:
                            // Start timer for 10 minutes
                            var timer = new System.Timers.Timer(600000); // 10 minutes in milliseconds
                            timer.Elapsed += (sender, e) =>
                            {
                                Console.WriteLine("Booking payment time has expired. Your booking is canceled.");
                                timer.Stop();
                                // If needed, you can cancel the booking here.
                            };
                            timer.Start();

                            Console.WriteLine("Booking will be canceled in 10 minutes if you don't complete payment.");
                            Console.WriteLine("Please enter your name: ");
                            string name = Console.ReadLine();
                            Console.WriteLine("Please enter your email: ");
                            string email = Console.ReadLine();

                            Console.WriteLine("Select an event to book:");
                            eventManager.ListEvents();

                            if (int.TryParse(Console.ReadLine(), out int selectedEventID))
                            {
                                if (eventManager.EventExists(selectedEventID))
                                {
                                    // List available seats for the selected event
                                    eventManager.ListAvailableSeats(selectedEventID);

                                    Console.WriteLine("Select up to 5 seats (comma-separated, e.g., 1,3,5): ");
                                    string selectedSeatsInput = Console.ReadLine();
                                    List<int> selectedSeats = selectedSeatsInput
                                        .Split(',')
                                        .Select(s => int.Parse(s))
                                        .ToList();

                                    if (selectedSeats.Count <= 5)
                                    {
                                        Console.WriteLine("Please choose a payment method:");
                                        Console.WriteLine("1. Invoice");
                                        Console.WriteLine("2. Direct Payment");
                                        Console.Write("Enter the number for your payment method: ");
                                        string paymentMethod = Console.ReadLine();
                                        if (paymentMethod == "1")
                                        {
                                            paymentMethod = "Invoice";
                                        }
                                        else if (paymentMethod == "2")
                                        {
                                            paymentMethod = "Direct Payment";
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid payment method choice.");
                                            return;
                                        }

                                        // Get Event information
                                        Event selectedEvent = eventManager.GetEventById(selectedEventID);

                                        Booking booking = bookingManager.CreateBooking(name, email, selectedEventID, selectedSeats, paymentMethod);

                                        Console.WriteLine($"Booking created with ID: {booking.BookingID}");
                                        Console.WriteLine($"Event: {selectedEvent.EventName}, Venue: {selectedEvent.Venue}");
                                    }
                                    else
                                    {
                                        Console.WriteLine("You can select up to 5 seats. Press Enter to continue.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid event selection. Press Enter to continue.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid event selection. Press Enter to continue.");
                            }
                            SaveData();
                            Console.WriteLine("Press Enter to continue.");
                            Console.ReadLine();
                            break;
                            
                            case 2:
                                Console.Write("Enter BookingID to cancel: ");
                                if (int.TryParse(Console.ReadLine(), out int bookingID))
                                {
                                    if (bookingManager.CancelBooking(bookingID))
                                    {
                                        Console.WriteLine("Booking canceled successfully.");
                                        Console.WriteLine("Press Enter to continue.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Booking not found or could not be canceled. Press Enter to continue.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid BookingID. Press Enter to continue.");
                                }

                                Console.ReadLine();
                                SaveData();
                                break;

                            case 3:
                                Console.WriteLine("Returning to the Main Menu.");
                                isUserMenuRunning = false;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                }
            }
        }

        private void AdminMenu()
        {
            bool isAdminMenuRunning = true;
            while (isAdminMenuRunning)
            {
                Console.Clear();

                Console.WriteLine("Admin Menu:");
                Console.WriteLine("1. Add Event");
                Console.WriteLine("2. Remove Event");
                Console.WriteLine("3. List Events");
                Console.WriteLine("4. Return to Main Menu");
                Console.Write("Select an option: ");

                if (int.TryParse(Console.ReadLine(), out int adminChoice))
                {
                    try
                    {
                        switch (adminChoice)
                        {
                            case 1:
                                    Console.Write("Enter Event Name: ");
                                    string eventName = Console.ReadLine();
                                    Console.Write("Enter Event Time: ");
                                    string eventTime = Console.ReadLine();
                                    Console.Write("Enter Event Date: ");
                                    string eventDate = Console.ReadLine();
                                    Console.Write("Enter Event Venue: ");
                                    string eventVenue = Console.ReadLine();

                                    Event newEvent = eventManager.CreateEvent(eventName, eventTime, eventDate, eventVenue);
                                    Console.WriteLine($"Event created with ID: {newEvent.EventID}");
                                    Console.WriteLine("Press Enter to continue.");
                                    Console.ReadLine();
                                    SaveData();
                                    break;


                            case 2:
                                Console.Write("Enter Event ID to remove: ");
                                if (int.TryParse(Console.ReadLine(), out int eventIDToRemove))
                                {
                                    if (eventManager.RemoveEvent(eventIDToRemove))
                                    {
                                        Console.WriteLine("Event removed successfully.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Event not found or could not be removed.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Event ID.");
                                }

                                Console.WriteLine("Press Enter to continue.");
                                Console.ReadLine();
                                SaveData();
                                break;

                            case 3:
                                Console.WriteLine("List of Events:");
                                eventManager.ListEvents();
                                Console.WriteLine("Press Enter to continue.");
                                Console.ReadLine();
                                break;

                            case 4:
                                Console.WriteLine("Returning to the Main Menu.");
                                isAdminMenuRunning = false;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                }
            }
        }

        public void SaveData()
            {
                eventManager.SaveData();
            }
        
    }
}