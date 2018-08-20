#version 440

layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec3 vNormal;

uniform mat4 model_matrix;
uniform mat4 view_matrix;
uniform mat4 projection_matrix;

out vec3 normalInterp;
out vec3 vertPos;

void main(){
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vPosition, 1.0);
    vec4 vertPos4 = view_matrix * model_matrix * vec4(vPosition, 1.0);

    normalInterp = vec3(transpose(inverse(view_matrix * model_matrix))) * vNormal;
    vertPos = vec3(vertPos4) / vertPos4.w;
}