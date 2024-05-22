
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
namespace Biljettshoppen
{
public class Event
    {
        public int EventID { get; set; }
        public string EventName { get; set; }
        public string Time { get; set; }
        public string Date { get; set; }
        public string Venue { get; set; }
        public List<int> AvailableSeats { get; set; }
        public List<int> UnavailableSeats { get; set; }
    }

    public class EventManager : IEventManager
    {
            private List<Event> events = new List<Event>();
            private string dataFile;
            private int nextEventID = 1; // Initialize the event ID counter
            private int totalSeatCount = 100; // Set the total seat count

            public EventManager(string dataFile)
            {
                this.dataFile = dataFile;
                LoadData();
            }

            public Event CreateEvent(string eventName, string eventTime, string eventDate, string eventVenue)
            {
                try
                {
                    LoadData();
                    Event newEvent = new Event
                    {
                        EventID = nextEventID,
                        EventName = eventName,
                        Time = eventTime,
                        Date = eventDate,
                        Venue = eventVenue,
                        AvailableSeats = Enumerable.Range(1, totalSeatCount).ToList(),
                        UnavailableSeats = new List<int>(),
                    };

                    events.Add(newEvent);
                    nextEventID++; // Increment the event ID

                    SaveData();

                    return newEvent;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while creating the event: " + ex.Message);
                    // You can log the exception or perform additional error handling here.
                    return null;
                }
            }


            public bool RemoveEvent(int eventID)
            {
                try
                {
                    LoadData();
                    Event eventToRemove = events.Find(e => e.EventID == eventID);
                    if (eventToRemove != null)
                    {
                        events.Remove(eventToRemove);
                        SaveData();
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while removing the event: " + ex.Message);
                    // You can log the exception or perform additional error handling here.
                    return false;
                }
            }

            public void ListEvents()
            {
                try
                {
                    LoadData();
                    Console.WriteLine("List of Events:");

                    if (File.Exists(dataFile))
                    {
                        string json = File.ReadAllText(dataFile);
                        var eventsFromFile = JsonSerializer.Deserialize<List<Event>>(json);

                        foreach (var e in eventsFromFile)
                        {
                            Console.WriteLine($"Event ID: {e.EventID}");
                            Console.WriteLine($"Event Name: {e.EventName}");
                            Console.WriteLine($"Time: {e.Time}");
                            Console.WriteLine($"Date: {e.Date}");
                            Console.WriteLine($"Venue: {e.Venue}");
                            Console.WriteLine("Available Seats: " + string.Join(", ", e.AvailableSeats));
                            Console.WriteLine("Unavailable Seats: " + string.Join(", ", e.UnavailableSeats));
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("No events found.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while listing events: " + ex.Message);
                    // You can log the exception or perform additional error handling here.
                }
            }

            public bool EventExists(int eventID)
            {
                try
                {
                    return events.Any(e => e.EventID == eventID);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while checking if the event exists: " + ex.Message);
                    // You can log the exception or perform additional error handling here.
                    return false;
                }
            }

            public void ListAvailableSeats(int eventID)
            {
                try
                {
                    Event selectedEvent = events.FirstOrDefault(e => e.EventID == eventID);
                    if (selectedEvent != null)
                    {
                        Console.WriteLine("Available Seats for Event:");
                        // You can add logic here to list available seats for the selected event.
                    }
                    else
                    {
                        Console.WriteLine("Event not found.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while listing available seats: " + ex.Message);
                    // You can log the exception or perform additional error handling here.
                }
            }
            public Event GetEventById(int eventID)
            {
                try
                {
                    return events.Find(e => e.EventID == eventID);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while getting the event by ID: " + ex.Message);
                    // You can log the exception or perform additional error handling here.
                    return null;
                }
            }

            public void SaveData()
            {
                try
                {
                    string json = JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(dataFile, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while saving data: " + ex.Message);
                    // You can log the exception or perform additional error handling here.
                }
            }

            public void LoadData()
            {
                try
                {
                    if (File.Exists(dataFile))
                    {
                        string json = File.ReadAllText(dataFile);
                        events = JsonSerializer.Deserialize<List<Event>>(json);
                        nextEventID = events.Count > 0 ? events.Max(e => e.EventID) + 1 : 1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while loading data: " + ex.Message);
                    // You can log the exception or perform additional error handling here.
                }
            }
        }
    }