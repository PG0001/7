"use client";
import React from "react";
import { Card, CardContent, Typography, Button, Box } from "@mui/material";

type Event = {
  EventId: number;
  UserId: number;
  EventName: string;
  Description?: string;
  StartTime: string;
  EndTime?: string;
};

export default function EventCard({ event }: { event: Event }) {
  const start = event.StartTime ? new Date(event.StartTime) : null;
  const end = event.EndTime ? new Date(event.EndTime) : null;

  return (
    <Card variant="outlined">
      <CardContent>
        <Typography variant="h6">{event.EventName}</Typography>
        {event.Description && (
          <Typography variant="body2" color="text.secondary" gutterBottom>
            {event.Description}
          </Typography>
        )}
        <Typography variant="caption" color="text.secondary">
          {start && end
            ? `${start.toLocaleString()} — ${end.toLocaleString()}`
            : start
            ? `${start.toLocaleString()}`
            : "Invalid Date"}
        </Typography>

        {/* Action Buttons */}
        <Box sx={{ display: "flex", gap: 1, mt: 2 }}>
          <Button
            variant="contained"
            size="small"
            sx={{ backgroundColor: "#415a77", "&:hover": { backgroundColor: "#627d98" } }}
            onClick={() => alert(`Notifications for ${event.EventName}`)}
          >
            Notifications
          </Button>
          <Button
            variant="outlined"
            size="small"
            sx={{ color: "#415a77", borderColor: "#415a77", "&:hover": { borderColor: "#627d98" } }}
            onClick={() => alert(`Edit ${event.EventName}`)}
          >
            Edit
          </Button>
        </Box>
      </CardContent>
    </Card>
  );
}
