"use client";
import React, { ReactNode } from "react";
import { Card, CardContent, Typography, Chip } from "@mui/material";

type Notification = {
  NotificationId: number;
  Title?: string | null;
  Message: string;
  Type: number; // enum 0=EMAIL,1=SMS,2=IN_APP
  Status?: string | null;
  CreatedAt?: string | null;
};

const getTypeString = (type: number) => {
  switch(type) {
    case 0: return "EMAIL";
    case 1: return "SMS";
    case 2: return "IN_APP";
    default: return "UNKNOWN";
  }
};

export default function NotificationCard({
  notification,
  children,
}: {
  notification: Notification;
  children?: ReactNode;
}) {
  const title = notification.Title ?? getTypeString(notification.Type);
  const body = notification.Message ?? "";

  return (
    <Card variant="outlined">
      <CardContent style={{ display: "flex", justifyContent: "space-between", gap: 12 }}>
        <div>
          <Typography variant="subtitle1">{title}</Typography>
          <Typography variant="body2" color="text.secondary">{body}</Typography>
          <Typography variant="caption" color="text.secondary">
            {notification.CreatedAt ? new Date(notification.CreatedAt).toLocaleString() : ""}
          </Typography>
          {children && <div style={{ marginTop: 8 }}>{children}</div>}
        </div>
        <div style={{ textAlign: "right" }}>
          {notification.Status && <Chip label={notification.Status} size="small" />}
          <div style={{ height: 8 }} />
          <Typography variant="caption">{getTypeString(notification.Type)}</Typography>
        </div>
      </CardContent>
    </Card>
  );
}
