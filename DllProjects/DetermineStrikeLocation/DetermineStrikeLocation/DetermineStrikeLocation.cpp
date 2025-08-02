#include "pch.h"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <cmath>
#include "DetermineStrikeLocation.h"

void bridge(const Vessel* vessel, Output* output) {
	constexpr double rad2Deg = 180.0/3.14159265358979323846; //why no pi constant c++

	//marshaling
	glm::vec2 outputVec;
	//output.x = 0;
	//output.y = 0;
	//output.angle = 0;

	glm::vec2 currentPoint(vessel->xCollider, vessel->yCollider);
	glm::vec2 velocity(vessel->vx, vessel->vy);
	glm::vec2 pastPoint = currentPoint - velocity;
	glm::vec2 collidedCenter(vessel->xCollided, vessel->yCollided);

	//determine chord intersection
	glm::vec2 d = currentPoint - pastPoint;
	glm::vec2 f = pastPoint - collidedCenter;

	float a = glm::dot(d, d);
	float b = 2 * glm::dot(f, d);
	float c = glm::dot(f, f) - (vessel->radius * vessel->radius);

	float discriminant = b * b - 4 * a * c;

	if (discriminant < 0)
		return;

	discriminant = std::sqrt(discriminant);

	float t1 = (-b - discriminant) / (2 * a);
	float t2 = (-b + discriminant) / (2 * a);

	if (t1 > 0 && t1 <= 1)
		outputVec = pastPoint + t1 * d;
	else if (t2 >= 0 && t2 <= 1)
		outputVec = pastPoint + t2 * d;

	output->x = outputVec.x;
	output->y = outputVec.y;
	output->angle = std::atan2(outputVec.y-collidedCenter.y, outputVec.x-collidedCenter.x) * rad2Deg;
}
