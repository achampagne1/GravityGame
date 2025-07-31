#include "pch.h"
#include <limits>
#include <cmath>
#include "ObjectMathEngine.h"

//I before E except after c
void recieveData(GravityPoint* gravityPoints, int gravityPointsLen) {
    gravityPointsInternal = gravityPoints;
    gravityPointsLenInternal = gravityPointsLen;
}

int bridge(GravityPoint self) {
    float closestGravityField = 10000.0;
    int out = 0;
    for (int i = 0; i < gravityPointsLenInternal; i++) {
        float adjustedDistance = magnitudeBetweenPoints(self, gravityPointsInternal[i]) / gravityPointsInternal[i].fieldSize;
        if (adjustedDistance < closestGravityField) {
            closestGravityField = adjustedDistance;
            out = i;
        }
    }
    return out;
}

float magnitudeBetweenPoints(GravityPoint a, GravityPoint b) {
    float dx = b.x - a.x;
    float dy = b.y - a.y;
    return std::sqrt((dx * dx) + (dy * dy));
}
