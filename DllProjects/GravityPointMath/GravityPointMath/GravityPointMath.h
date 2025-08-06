#pragma once

struct GravityPoint
{
    float x;
    float y;
    float fieldSize;
};

std::vector<GravityPoint*> gravityPoints;

extern "C" __declspec(dllexport) void addGravityPoint(GravityPoint* gravityPoint);
extern "C" __declspec(dllexport) void removeGravityPoint(GravityPoint* gravityPoint);
extern "C" __declspec(dllexport) void clearVec();
extern "C" __declspec(dllexport) GravityPoint** returnVec(int* size);
extern "C" __declspec(dllexport) GravityPoint calulateClosestField(GravityPoint* self);
float magnitudeBetweenPoints(GravityPoint* a, GravityPoint* b);