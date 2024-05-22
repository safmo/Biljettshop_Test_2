namespace Biljettshoppen
{
    public interface IEventManager
    {
        Event CreateEvent(string eventName, string eventTime, string eventDate, string eventVenue);
        bool RemoveEvent(int eventID);
        void ListEvents();
        bool EventExists(int eventID);
        void ListAvailableSeats(int eventID);
        Event GetEventById(int eventID);
        void SaveData();
    }
}
