// app/events/create/page.tsx
"use client";
import React, { useState } from "react";
import { Container, TextField, Button, Typography, Box } from "@mui/material";
import { useRouter } from "next/navigation";
import { createEvent } from "../../services/api";
import Layout from "../../ui/layout";

export default function CreateEventPage() {
  const [form, setForm] = useState({ eventName: "", description: "", startTime: "", endTime: "" });
  const router = useRouter();
  const userId = 1;

  const submit = async (e:any) => {
    e.preventDefault();
    try {
      await createEvent({UserId: userId, ...form });
      alert("Event created");
      router.push("/events");
    } catch (err:any) {
      console.error(err);
      alert(err.message || "Failed");
    }
  };

  return (
    <Layout>
    <Container sx={{ py:4 }}>
      <Typography variant="h5" mb={2}>Create Event</Typography>
      <Box component="form" onSubmit={submit} sx={{ display: "grid", gap: 2, maxWidth: 600 }}>
        <TextField label="Event Name" value={form.eventName} onChange={e => setForm({...form, eventName: e.target.value})} required />
        <TextField label="Description" value={form.description} onChange={e => setForm({...form, description: e.target.value})} multiline />
        <TextField label="Start Time" type="datetime-local" InputLabelProps={{ shrink: true }} value={form.startTime} onChange={e => setForm({...form, startTime: e.target.value})} required />
        <TextField label="End Time" type="datetime-local" InputLabelProps={{ shrink: true }} value={form.endTime} onChange={e => setForm({...form, endTime: e.target.value})} required />
        <Button type="submit" variant="contained">Create</Button>
      </Box>
    </Container>
    </Layout>
  );
}
