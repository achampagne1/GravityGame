#pragma once

struct GravityPoint
{
    float x;
    float y;
    float fieldSize;
};

GravityPoint* gravityPointsInternal = nullptr;
int gravityPointsLenInternal = 0;

extern "C" __declspec(dllexport) void recieveData(GravityPoint* gravityPoints, int gravityPointsLen);

extern "C" __declspec(dllexport) int bridge(GravityPoint self);

float magnitudeBetweenPoints(GravityPoint a, GravityPoint b);