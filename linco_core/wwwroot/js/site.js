// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    // Harf butonları için seçim mantığı (sadece bir harf seçilebilir)
    $('.alpha-btn').on('click', function () {
        $('.alpha-btn').removeClass('active');
        $(this).addClass('active');
    });

    // Seviye butonları için seçim mantığı (sadece bir seviye seçilebilir)
    $('.level-btn').on('click', function () {
        $('.level-btn').removeClass('active');
        $(this).addClass('active');
    });
});
