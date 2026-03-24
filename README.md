# Tactical Radar Simulator (WPF)

![Radar Simulation](Tactical-Radar-Simulator-WPF.gif)

## Overview
A dynamic, real-time tactical radar simulation engine built from scratch using **C#** and **WPF**. Designed as a Proof of Concept (PoC) for Early Warning (EW) display systems, this project bridges the gap between raw mathematical data processing and high-performance visual rendering. It simulates a Plan Position Indicator (PPI) radar screen capable of tracking multiple dynamic entities in a continuous rendering loop.

At its core, the simulation engine relies on **Trigonometric Vector Mathematics**. It continuously calculates and converts incoming polar coordinate data (angle and distance) into precise Cartesian coordinates (X, Y) to update the visual interface. To mimic real-world hardware refresh rates without freezing the UI thread, the system utilizes a polite, asynchronous 50Hz update loop (`DispatcherTimer`), ensuring smooth 60FPS-like performance even when processing swarms of targets.

Beyond mere visualization, the project implements fundamental **Command and Control (C2)** logic. It features an object-oriented architecture where each radar target acts as an independent entity with its own kinematic properties (speed, trajectory, evasion probability). A real-time threat assessment algorithm constantly evaluates each target's distance, dynamically reclassifying and color-coding threats (from Yellow to Red) as they breach critical defense perimeters, complete with live HUD data tags.

## Key Features
* **Polar to Cartesian Conversion:** Uses Trigonometry (Sin/Cos) to translate angles and distances into X,Y screen coordinates.
* **Dynamic Target Tracking:** Simulates multiple moving entities (swarms) simultaneously using optimized data structures (`List`).
* **Threat Assessment Engine:** Implements a conditional zoning system to classify targets (Yellow for warning, Red for imminent danger) based on radius penetration.
* **Live HUD Data Tags:** Attaches real-time distance readouts to moving targets.
* **Kinematic Evasion Logic:** Targets utilize a pseudo-random probability matrix to perform slight evasive maneuvers (course alterations) instead of static linear movement.

## Tech Stack
* **Language:** C#
* **Framework:** .NET / WPF (Windows Presentation Foundation)
* **Architecture:** Event-driven UI with a 50Hz Update Loop (DispatcherTimer)

## Author
Muhammed Cemal Aldahruc - Computer Engineering Student
