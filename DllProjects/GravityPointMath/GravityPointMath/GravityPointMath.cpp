#include "pch.h"
#include <limits>
#include <cmath>
#include <vector>
#include "GravityPointMath.h"

void addGravityPoint(GravityPoint* gravityPoint) {
	gravityPoints.push_back(gravityPoint);
}

void removeGravityPoint(GravityPoint* gravityPoint) {
    gravityPoints.erase(
        std::remove(gravityPoints.begin(), gravityPoints.end(), gravityPoint),
        gravityPoints.end()
    );
}

GravityPoint calulateClosestField(GravityPoint* self) {
    float closestGravityField = 10000.0;
    GravityPoint* gravityPoint = nullptr;
    for (int i = 0; i < gravityPoints.size(); i++) {
        float adjustedDistance = magnitudeBetweenPoints(self, gravityPoints[i]) / gravityPoints[i]->fieldSize;
        if (adjustedDistance < closestGravityField) {
            closestGravityField = adjustedDistance;
            gravityPoint = gravityPoints[i];
        }
    }
    return *gravityPoint;
}

float magnitudeBetweenPoints(GravityPoint* a, GravityPoint* b) {
    float dx = b->x - a->x;
    float dy = b->y - a->y;
    return std::sqrt((dx * dx) + (dy * dy));
};