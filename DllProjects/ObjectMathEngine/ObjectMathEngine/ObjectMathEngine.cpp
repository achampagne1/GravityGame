#include "pch.h"
#include <limits>
#include <cmath>
#include "ObjectMathEngine.h"

int bridge(GravityPoint self, GravityPoint* gravityPoints, int gravityPointsLen) {
    float closestGravityField = 10000.0;
    int out = 0;
    for (int i = 0; i < gravityPointsLen; i++) {
        float adjustedDistance = magnitudeBetweenPoints(self, gravityPoints[i]) / gravityPoints[i].fieldSize;
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

/*
float closestGravityField = 1000f;
            GameObject temp = gravityPoints[0];
            foreach (GameObject gravityPoint in gravityPoints)
            {
                GravityPointController gravityPointController = gravityPoint.GetComponent<GravityPointController>();
                float adjustedDistance = (float)(transform.position - gravityPoint.transform.position).magnitude / gravityPointController.getFieldSize();
                if (adjustedDistance < closestGravityField)
                {
                    closestGravityField = adjustedDistance;
                    temp = gravityPoint;
                }
            }*/