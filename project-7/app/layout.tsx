import type { Metadata } from "next";
import "./globals.css";
import EmotionRegistry from "../app/emotionregistry";



export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (<html> 
    <body>{children}</body></html>);
  
}
