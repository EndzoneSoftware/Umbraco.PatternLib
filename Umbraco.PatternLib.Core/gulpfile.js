/// <binding ProjectOpened='default' />
var gulp = require('gulp');
var sass = require('gulp-sass');
var gulputil = require('gulp-util');
var path = require('path');
var merge = require('merge-stream');
var sourcemaps = require('gulp-sourcemaps');
var postcss = require('gulp-postcss');
var autoprefixer = require('autoprefixer');
var discardcomments = require('postcss-discard-comments');
var csso = require('gulp-csso');
var babel = require('gulp-babel');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');

var handleError = function (err) {
    gulputil.log(gulputil.colors.red('[Error]'), err.toString());
};

var sassPaths = [
    __dirname, '../node_modules/foundation-sites/scss', '../node_modules/foundation-icon-fonts'
];

gulp.task('sass', function () {
    gulp.src(['_frontend/scss/styles.scss'])
      .pipe(sourcemaps.init())
      .pipe(sass({
          includePaths: sassPaths
      })
        .on('error', sass.logError))
      .pipe(postcss([
          autoprefixer({
              browsers: ['last 2 versions', 'ie > 8']
          }),
          discardcomments({ removeAll: true })]
      ))      
      .pipe(csso())
      .pipe(sourcemaps.write('.'))
      .pipe(gulp.dest('App_Plugins/Umbraco.PatternLib/assets/css'));
});

gulp.task('copy', function () {
    gulp.src('../node_modules/jquery/dist/jquery.js')
        .pipe(gulp.dest('_frontend/scripts/vendor/jquery'))
    gulp.src(['../node_modules/foundation-sites/dist/js/plugins/foundation.core.js',
              '../node_modules/foundation-sites/dist/js/plugins/foundation.util.mediaQuery.js',
              '../node_modules/foundation-sites/dist/js/plugins/foundation.util.keyboard.js',
              '../node_modules/foundation-sites/dist/js/plugins/foundation.util.box.js',
              '../node_modules/foundation-sites/dist/js/plugins/foundation.util.nest.js',
              '../node_modules/foundation-sites/dist/js/plugins/foundation.util.triggers.js',
              '../node_modules/foundation-sites/dist/js/plugins/foundation.util.motion.js',
              '../node_modules/foundation-sites/dist/js/plugins/foundation.sticky.js',
              '../node_modules/foundation-sites/dist/js/plugins/foundation.dropdownMenu.js',
              '../node_modules/foundation-sites/dist/js/plugins/foundation.offcanvas.js',
              '../node_modules/foundation-sites/dist/js/plugins/foundation.toggler.js'])
        .pipe(gulp.dest('_frontend/scripts/vendor/foundation-sites'))
    gulp.src(['../node_modules/foundation-icon-fonts/foundation-icons.eot',
              '../node_modules/foundation-icon-fonts/foundation-icons.svg',
              '../node_modules/foundation-icon-fonts/foundation-icons.ttf',
              '../node_modules/foundation-icon-fonts/foundation-icons.woff'])
        .pipe(gulp.dest('App_Plugins/Umbraco.PatternLib/assets/fonts/icons'))
    gulp.src(['../node_modules/prismjs/components/prism-core.js',
              '../node_modules/prismjs/components/prism-markup.js',
              '../node_modules/prismjs/plugins/line-numbers/prism-line-numbers.js'])
        .pipe(gulp.dest('_frontend/scripts/vendor/prismjs'));
});

gulp.task('js', function () {
    gulp.src(['_frontend/scripts/vendor/jquery/jquery.js'])
        .pipe(sourcemaps.init())
        .pipe(concat('jquery.js'))
        .pipe(uglify())
        .on('error', handleError)
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest('App_Plugins/Umbraco.PatternLib/assets/scripts'))
    gulp.src(['_frontend/scripts/vendor/foundation-sites/foundation.core.js',
              '_frontend/scripts/vendor/foundation-sites/foundation.util.mediaQuery.js',
              '_frontend/scripts/vendor/foundation-sites/foundation.util.keyboard.js',
              '_frontend/scripts/vendor/foundation-sites/foundation.util.box.js',
              '_frontend/scripts/vendor/foundation-sites/foundation.util.nest.js',
              '_frontend/scripts/vendor/foundation-sites/foundation.util.triggers.js',
              '_frontend/scripts/vendor/foundation-sites/foundation.util.motion.js',
              '_frontend/scripts/vendor/foundation-sites/foundation.sticky.js',
              '_frontend/scripts/vendor/foundation-sites/foundation.dropdownMenu.js',
              '_frontend/scripts/vendor/foundation-sites/foundation.offcanvas.js',
              '_frontend/scripts/vendor/foundation-sites/foundation.toggler.js',
              '_frontend/scripts/vendor/prismjs/prism-core.js',
              '_frontend/scripts/vendor/prismjs/prism-markup.js',
              '_frontend/scripts/vendor/prismjs/prism-line-numbers.js',
              '_frontend/scripts/foundation.js',
              '_frontend/scripts/viewer.js'])
        .pipe(sourcemaps.init())
        .pipe(concat('scripts.js'))
        .pipe(uglify())
        .on('error', handleError)
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest('App_Plugins/Umbraco.PatternLib/assets/scripts'));
});

gulp.task('default', ['sass'], function () {
    gulp.watch(['_frontend/scss/**/*.scss'], ['sass']);
    gulp.watch(['_frontend/scripts/**/*.js'], ['js']);
});
