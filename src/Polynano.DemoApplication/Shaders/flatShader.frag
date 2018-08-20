#version 440
precision mediump float;

in vec3 vertPos;
in vec3 normalInterp;

out vec4 fragColor;


uniform vec3 mesh_color;

const vec3 lightPos  = vec3(200,60,250);
const vec3 specColor  = vec3(1.0, 1.0, 1.0);

void main() {

 vec3 ambientColor = mesh_color * 0.4;
 vec3 normal = mix(normalize(normalInterp), normalize(cross(dFdx(vertPos), dFdy(vertPos))), 0.9999999f);
 vec3 lightDir = normalize(lightPos - vertPos);

 float lambertian = max(dot(lightDir,normal), 0.0);
 float specular = 0.0;

 if(lambertian > 0.0) {
  vec3 viewDir = normalize(-vertPos);
  vec3 halfDir = normalize(lightDir + viewDir);
  float specAngle = max(dot(halfDir, normal), 0.0);
  specular = pow(specAngle, 16.0);
 }

 fragColor = vec4(ambientColor + lambertian * mesh_color + specular * specColor, 1.0);
}