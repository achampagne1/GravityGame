#pragma once

struct GravityPoint
{
    float x;
    float y;
    float fieldSize;
};

extern "C" __declspec(dllexport) int bridge(GravityPoint self, GravityPoint* gravityPoints, int gravityPointsLen);

float magnitudeBetweenPoints(GravityPoint a, GravityPoint b);