"use client";

import React, { ReactNode } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import {
  Box,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Divider,
} from "@mui/material";

export default function Layout({ children }: { children: ReactNode }) {
  const pathname = usePathname();

  const menuItems = [
    { label: "Dashboard", path: "/" },
    { label: "Events", path: "/events" },
    { label: "Notifications", path: "/notifications" },
    { label: "Templates", path: "/Templates" },
    { label: "Settings", path: "/settings" },
  ];

  return (
    <Box sx={{ display: "flex", minHeight: "100vh" }}>
      {/* SIDEBAR */}
      <Box
        sx={{
          width: 240,
          bgcolor: "#0d1b2a", // dark navy background
          color: "white",
          p: 2,
        }}
      >
        <List>
          {menuItems.map((item) => (
            <ListItem key={item.path} disablePadding>
              <ListItemButton
                component={Link}
                href={item.path}
                sx={{
                  color: "white",
                  borderRadius: 2,
                  mb: 0.5,
                  backgroundColor:
                    pathname === item.path ? "#1b263b" : "transparent",
                  "&:hover": {
                    backgroundColor: "#415a77",
                  },
                }}
              >
                <ListItemText primary={item.label} />
              </ListItemButton>
            </ListItem>
          ))}
        </List>

        <Divider sx={{ my: 2, borderColor: "rgba(255,255,255,0.2)" }} />

        <List>
          <ListItem disablePadding>
            <ListItemButton
              component={Link}
              href="/notifications/send"
              sx={{
                color: "white",
                borderRadius: 2,
                backgroundColor:
                  pathname === "/notifications/send"
                    ? "#1b263b"
                    : "transparent",
                "&:hover": {
                  backgroundColor: "#415a77",
                },
              }}
            >
              <ListItemText primary="Send Notification" />
            </ListItemButton>
          </ListItem>
        </List>
      </Box>

      {/* MAIN CONTENT */}
      <Box sx={{ flex: 1, p: 3, bgcolor: "#f5f6fa" }}>{children}</Box>
    </Box>
  );
}
