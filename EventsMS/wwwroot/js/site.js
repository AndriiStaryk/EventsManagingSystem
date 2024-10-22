// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


var map = L.map('map').setView([50.4501, 30.5234], 6);

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 18,
    attribution: '© OpenStreetMap contributors'
}).addTo(map);

fetch('/Events/GetLocations')
    .then(response => response.json())
    .then(data => {
        data.forEach(function (location) {
            L.marker([location.lat, location.lng])
                .addTo(map)
                .bindPopup(location.EventName);
            console.log(location.EventName);
        });
    })
    .catch(error => console.log('Error fetching locations:', error));