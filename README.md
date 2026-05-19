# BLOG

<img width="493" height="496" alt="image" src="https://github.com/user-attachments/assets/5944b84d-0d04-42dd-a465-1943a25f2778" />

# 8th of May – Rendering a sphere
The aim of this project is to create a shader for rendering procedurally generated photo-realistic planets in real time. The project is being built in Unity but is using none of its built-in rendering functionality. Instead, Unity was chosen for its ease of use when creating shaders. The first step was to set up a compute shader and have it write to an image displayed on the screen, which went smoothly. 

The next step was using this shader to render a sphere using raytracing. The technique used is based on the one learnt in our second DGI lab of the rendering track. The technique models a pinhole camera where a ray is cast from the focal point to each pixel on the screen and onwards, and its intersection with geometry in the scene determines its color. In the DGI lab, the geometry was made up of polygons while this project will use a mathematical sphere. The planet could be described using polygons, this is the usual approach for rendering planets with actual height differences in geometry. The goal of this project, however, is to render planets of realistic scale viewed from space where height differences are negligible compared to the radius of the planets. In this case, describing the planet as a smooth mathematical sphere and using normal maps to convey height will probably suffice. This avoids the hassle of having to subdivide a sphere into polygons. The intersection of a ray and the sphere is found by solving where the ray satisfies the equation of the sphere, i.e. all points at a fixed distance from its center. The normal of each intersection point can then be calculated by subtracting the planet center position from this point and normalizing the resulting vector. 

Lighting the sphere was then very simple. The illumination of each point is calculated using the dot product of the point normal and the direction from the point to the sun. If the normal points towards the sun, the angle between the normal and the sun direction is 0 which gives a dot product of one, thereby resulting in maximum illumination. An angle of 90 degrees which occurs on the rim between the day and the night side of the planet, the so-called terminator, gives a dot product of 0 and thereby no illumination. An angle larger than 90 degrees gives negative values and is also set to no illumination, thereby creating a night side of the planet. This results in the lit sphere visible above, which is currently rendered at about 800 frames per second on my desktop computer. This is hopefully a large enough performance margin to eventually be able to render complex terrain and an atmosphere. 

---

<img width="48%" height="445" alt="image" src="https://github.com/user-attachments/assets/1a2cb3c9-775d-4bc9-99a6-8cd03d1e6f3a" /> <img width="48%" height="445" alt="image" src="https://github.com/user-attachments/assets/3065449e-2687-4989-91b4-3bfea108b74e" />

# 9th of May – Implementing noise

The planet terrain and normals will be generated using noise. Generating a 2D texture and wrapping it around a sphere comes with some difficulties (see map projection). This project will circumvent this by instead sampling 3D noise functions, using the intersection point of each ray as input and returning a float that can be used to determine, for example, height. In other words, the terrain of the planet is determined by the sphere’s intersection of 3D noise. This idea was inspired by a video by the channel Fractal Philosophy. He has divided his planet into tiles and is sampling noise per tile, while this project will sample noise for each intersection point, i.e. for each pixel.

For the noise, I implemented a gradient noise function described in an article by Inigo Quilez. It generates smooth 3D noise as well as its gradient. The gradient of the noise is then used to shade the terrain by having it tilt the normal that is used for the illumination calculation, which results in the terrain visible above on the left. 

Next up, I implemented so-called fractal Brownian motion, also from an article by Inigo Quilez. It is a technique of layering noise in a way that creates very natural looking self-similar structures, where incrementally smaller detail is added to the terrain. It works by adding together multiple noise functions, in this case called octaves, where each octave has double the frequency and a reduced amplitude compared to the previous one. This results in terrain made up of a few large structures, more medium size structures and a lot of smaller structures. 

Finally, I implemented another noise related technique called domain warping. As with the previous two techniques, this one is also based on an article by Inigo. Domain warping can be used to twist and bend noise, thereby breaking up otherwise repetitive patterns. It works by using noise functions to offset the sampling position of another noise function, effectively warping the domain before evaluating the final noise. This creates more natural and irregular structures, as seen in the image above to the right.

Noise sampling: [Maps: Fractals, Tectonics and the Fourth Dimension](https://www.youtube.com/watch?v=7xL0udlhnqI&t=675s)

Gradient noise: [Inigo Quilez :: computer graphics, maths, shaders, fractals, demoscene](https://iquilezles.org/articles/gradientnoise/)

Fractal Brownian motion: [Inigo Quilez :: computer graphics, maths, shaders, fractals, demoscene](https://iquilezles.org/articles/fbm/)

Domain warping: [Inigo Quilez :: computer graphics, maths, shaders, fractals, demoscene](https://iquilezles.org/articles/warp/)

---
 
<img width="471" height="470" alt="image" src="https://github.com/user-attachments/assets/51e53444-1c05-4739-af7e-da75f1dc5d50" /> <img src="preview.gif" width="48%" />

# 10th of May – Controlling the camera
The image above is the result of tuning the techniques I implemented in the previous blog, to achieve realistic and natural terrain. However, this is not the topic of today’s blog. 
Today I added the ability to control the camera that views the planet. I wanted to create controls similar to those of Google Earth, i.e. panning the camera horizontally and vertically around the planet as well as zoom. The panning is using a spherical coordinate system to orbit around the planet while always pointing towards its center. The zoom works by changing the distance of the camera from the planet. The camera distance decreases exponentially, thereby getting closer to the surface but never reaching it. The panning speed is scaled with both the zoom level and the latitude so that the movement feels consistent across the planet. 
The ability to zoom really shows the fractal Brownian motion that I implemented in the last blog, where details of smaller and smaller scale present itself the further you zoom. This can be viewed in the video above.

---

<img width="499" height="509" alt="image" src="https://github.com/user-attachments/assets/72ec6e57-af79-408e-bb09-507009aaa592" />

# 15th of May – Coloring the planet
