import urllib.request
from multiprocessing import Pool
import os.path

request_string = "https://mars.nasa.gov/maps/explore-mars-map/catalog/Mars_Viking_MDIM21_ClrMosaic_global_232m/1.0.0/default/default028mm/{}/{}/{}.png"


def main():
    get_mars()


def get_image(coords):
    (x, y, z) = coords
    filename = 'tiles/{}_{}_{}.png'.format(z, x, y)
    if not os.path.isfile(filename):
        urllib.request.urlretrieve(request_string.format(z, x, y), filename)


def get_mars():
    tiles = [(c, r, 9) for c in range(0, 512) for r in range(0, 256)]
    p = Pool(8)
    p.map(get_image, tiles)


if __name__ == "__main__":
    main()
