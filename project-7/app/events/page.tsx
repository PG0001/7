"use client";

import React, { useEffect, useState } from "react";
import { Container, Typography, Button, Box, Paper, Grid } from "@mui/material";
import EventCard from "@/Components/EventCard";
import { fetchEvents, createEvent } from "@/app/services/api";
import Layout from "../ui/layout";
import { useRouter } from "next/navigation";

type Event = {
  EventId: number;
  UserId: number;
  EventName: string;
  Description?: string;
  StartTime: string;
  EndTime?: string;
};

export default function EventsPage() {
  const [events, setEvents] = useState<Event[]>([]);
  const userId = 1;
  const router = useRouter();

  // Load events from backend
  const loadEvents = async () => {
    try {
      const data = await fetchEvents(userId);
      console.log("Fetched events:", data);
      setEvents(data);
    } catch (err) {
      console.error(err);
      alert("Failed to load events");
    }
  };

  useEffect(() => {
    loadEvents();
  }, []);

  // Quick-create a new event
  const onCreateEvent = async () => {
    try {
      const now = new Date();
      const start = new Date(now.getTime() + 1000 * 60 * 60); // 1 hour later
      const end = new Date(now.getTime() + 1000 * 60 * 60 * 2); // 2 hours later

      await createEvent({
        UserId: userId,
        eventName: `Event ${now.toLocaleTimeString()}`,
        description: "Created from frontend",
        startTime: start.toISOString(),
        endTime: end.toISOString(),
      });

      await loadEvents();
      alert("Event created. Backend will schedule reminders.");
    } catch (err: any) {
      console.error(err);
      alert(err.message || "Failed to create event");
    }
  };

  return (
    <Layout>
      <Container sx={{ py: 4 }}>
        {/* Header */}
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 3,
            bgcolor: "#1b263b",
            color: "white",
            p: 2,
            borderRadius: 2,
            boxShadow: 3,
          }}
        >
          <Typography variant="h5">Events</Typography>
          <Box>
            <Button
              variant="contained"
              sx={{
                mr: 2,
                backgroundColor: "#415a77",
                "&:hover": { backgroundColor: "#627d98" },
              }}
              onClick={onCreateEvent}
            >
              Quick Event
            </Button>
            <Button
              variant="contained"
              sx={{
                backgroundColor: "#415a77",
                "&:hover": { backgroundColor: "#627d98" },
              }}
              onClick={() => router.push("/events/create")}
            >
              Create Event
            </Button>
          </Box>
        </Box>

   {/* Event Grid */}
<Grid container spacing={3}>
  {events.length === 0 && (
    <Grid size={12}>
      <Paper sx={{ p: 3, textAlign: "center", bgcolor: "#e0e1e5" }}>
        No events yet. Click "Create Quick Event" to add one.
      </Paper>
    </Grid>
  )}

  {events.map((ev) => (
    <Grid key={ev.EventId} size={{ xs: 12, md: 6 }}>
      <EventCard event={ev} />
    </Grid>
  ))}
</Grid>

      </Container>
    </Layout>
  );
}
