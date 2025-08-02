#pragma once

struct Vessel
{
    float vx;
    float vy;
    float xCollider;
    float yCollider;
    float radius;
    float xCollided;
    float yCollided;
};

struct Output
{
    float x;
    float y;
    float angle;
};

extern "C" __declspec(dllexport) void bridge(const Vessel* vessel, Output* output);