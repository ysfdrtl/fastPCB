import type { Project, Report } from "./types";

export const demoProjects: Project[] = [
  {
    id: 9001,
    userId: 101,
    title: "STM32 Motor Driver Board",
    description: "Kucuk robot platformlari icin gelistirilmis iki katmanli motor surucu karti. Header yerlesimi ve guc izi dagilimi ogrenme odakli tutuldu.",
    filePath: "",
    technicalDetails: {
      layers: 2,
      material: "FR4",
      minDistance: 0.2,
      quantity: 5
    },
    createdAt: "2026-04-01T10:30:00Z",
    updatedAt: "2026-04-01T10:30:00Z",
    status: "Published",
    owner: {
      id: 101,
      firstName: "Ayse",
      lastName: "Kaya"
    }
  },
  {
    id: 9002,
    userId: 102,
    title: "ESP32 Sensor Hub",
    description: "Birden fazla I2C sensoru tek kartta toplayan deneysel hub tasarimi. Duzgun konnektor yerlesimi ve kolay lehimlenebilir footprint secimleri hedeflendi.",
    filePath: "",
    technicalDetails: {
      layers: 4,
      material: "Rogers",
      minDistance: 0.15,
      quantity: 3
    },
    createdAt: "2026-04-02T14:10:00Z",
    updatedAt: "2026-04-02T14:10:00Z",
    status: "Featured",
    owner: {
      id: 102,
      firstName: "Mert",
      lastName: "Demir"
    }
  },
  {
    id: 9003,
    userId: 103,
    title: "USB-C Power Breakout",
    description: "USB-C guc dagitimi icin hazirlanan basit breakout karti. Hobi projelerinde hizli entegrasyon icin test pad ve genis guc hatlari eklendi.",
    filePath: "",
    technicalDetails: {
      layers: 2,
      material: "Aluminum",
      minDistance: 0.25,
      quantity: 10
    },
    createdAt: "2026-04-03T09:00:00Z",
    updatedAt: "2026-04-03T09:00:00Z",
    status: "Published",
    owner: {
      id: 103,
      firstName: "Elif",
      lastName: "Sahin"
    }
  }
];

export const demoReports: Report[] = [
  {
    id: 8001,
    projectId: 9002,
    userId: 1,
    reason: "Eksik lisans bilgisi",
    details: "Aciklama guclu ama paylasilan dosyanin lisans notu eklenmeli gibi gorunuyor.",
    status: "Open",
    response: "",
    createdAt: "2026-04-04T13:00:00Z",
    updatedAt: "2026-04-04T13:00:00Z"
  }
];
