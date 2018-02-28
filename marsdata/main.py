import urllib.request
from multiprocessing import Pool
import os.path
import cv2
import numpy as np
request_string = "https://mars.nasa.gov/maps/explore-mars-map/catalog/Mars_Viking_MDIM21_ClrMosaic_global_232m/1.0.0/default/default028mm/{}/{}/{}.png"


def main():
    get_mars()
    #load_height()

def load_height():
    fn = "Mars_MGS_MOLA_DEM_mosaic_global_463m.tif"
    im = cv2.imread(fn)
    print(np.max(im))


def get_image(coords):
    (x, y, z) = coords
    filename = 'tiles/{}_{}_{}.png'.format(z, x, y)
    if not os.path.isfile(filename):
        urllib.request.urlretrieve(request_string.format(z, x, y), filename)


def get_mars():
    tiles = [(c, r, 0) for c in range(0, 4) for r in range(0, 4)]
    for tile in tiles:
        get_image(tile)
    #p = Pool()
    #p.map(get_image, tiles)


if __name__ == "__main__":
    main()
