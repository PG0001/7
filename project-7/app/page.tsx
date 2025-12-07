"use client";

import { useEffect, useState } from "react";
import { fetchEvents, fetchNotifications } from "../app/services/api";
import EventCard from "@/Components/EventCard";
import NotificationCard from "@/Components/NotificationCard";
import Layout from "./ui/layout";

export default function DashboardPage() {
  const [events, setEvents] = useState<any[]>([]);
  const [notifications, setNotifications] = useState<any[]>([]);

  useEffect(() => {
    fetchEvents(1).then(setEvents);
    fetchNotifications(1).then(setNotifications);
  }, []);

  return (
    <Layout>
      <div className="p-6 bg-gray-100 min-h-screen">
        <h1 className="text-3xl font-bold mb-6 text-gray-800">Dashboard</h1>

        {/* Two-column layout */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {/* Left Column: Upcoming Events */}
          <section className="bg-white rounded-lg shadow-md p-4">
            <h2 className="text-2xl font-semibold mb-4 text-blue-600">Upcoming Events</h2>
            <div className="space-y-4">
              {events.length === 0 && <p className="text-gray-500">No upcoming events.</p>}
              {events.map((e: any) => (
                <EventCard key={e.EventId} event={e} />
              ))}
            </div>
          </section>

          {/* Right Column: Notifications */}
          <section className="bg-white rounded-lg shadow-md p-4">
            <h2 className="text-2xl font-semibold mb-4 text-green-600">Notifications</h2>
            <div className="space-y-4">
              {notifications.length === 0 && <p className="text-gray-500">No notifications.</p>}
              {notifications.map((n: any) => (
                <NotificationCard key={n.NotificationId} notification={n} />
              ))}
            </div>
          </section>
        </div>
      </div>
    </Layout>
  );
}
